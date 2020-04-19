using System;
using System.Collections.Generic;
using System.Linq;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Models;

namespace WarOfGalaxiesApi.Controllers
{
    public static class VerifyController
    {

        /// <summary>
        /// Doğrulananların listesi.
        /// </summary>
        public static List<int> Verifies = new List<int>();

        /// <summary>
        /// Gezegene ait kaynakları kullanmadan önce çağrılması gereken methot.
        /// Bütün kaynak hesaplamalarını yeniden yapar.
        /// </summary>
        /// <param name="verifyData"></param>
        /// <returns></returns>
        public static bool VerifyPlanetResources(IUnitOfWork uow, VerifyResourceDTO verifyData)
        {

            #region Doğrulamadan önce zaten doğruluyor mu diye kontrol ediyoruz.

            lock (Verifies)
            {
                // Eğer zaten listede ve verify ediliyor ise hata dön.
                if (Verifies.Contains(verifyData.UserPlanetID))
                    throw new Exception("Zaten bir doğrulama yapılıyor.");

                // Listeye ekliyoruz.
                Verifies.Add(verifyData.UserPlanetID);
            }

            #endregion

            try
            {
                // Sistem tarihi.
                DateTime currentDate = DateTime.UtcNow;

                #region Kullanıcının gezegeni ve gezegendeki binalar ve binaların yükseltmeleri

                // Kullanıcının gezegenini buluyoruz.i
                TblUserPlanets userPlanet = uow.GetRepository<TblUserPlanets>().FirstOrDefault(x => x.UserPlanetId == verifyData.UserPlanetID);

                // Gezegendeki binaları buluyoruz.
                List<TblUserPlanetBuildings> userPlanetBuildings = uow.GetRepository<TblUserPlanetBuildings>().Where(x => x.UserPlanetId == verifyData.UserPlanetID).ToList();

                // Gezegendeki yükseltmeleri buluyoruz.
                List<TblUserPlanetBuildingUpgs> userPlanetBuildingUpgs = uow.GetRepository<TblUserPlanetBuildingUpgs>().Where(x => x.UserPlanetId == verifyData.UserPlanetID).ToList();

                #endregion

                #region Metal Üretimi
                {
                    // En son güncellemeden bu yana geçen süreyi buluyoruz.
                    double passedSeconds = (currentDate - userPlanet.LastUpdateDate).TotalSeconds;

                    #region Kaynak Binaları

                    // Gezegendeki kaynak deposunu buluyoruz..
                    TblUserPlanetBuildings metalStorageBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)Buildings.MetalDeposu);

                    // Gezegendeki kaynak deposunun yükseltmesini buluyoruz.
                    TblUserPlanetBuildingUpgs metalStorageBuildingUpg = userPlanetBuildingUpgs.Find(x => x.BuildingId == (int)Buildings.MetalDeposu);

                    // Gezegendeki kaynak binasını buluyoruz.
                    TblUserPlanetBuildings metalBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)Buildings.MetalMadeni);

                    // Gezegendeki kaynak binasının yükseltmesini buluyoruz.
                    TblUserPlanetBuildingUpgs metalBuildingUpg = userPlanetBuildingUpgs.Find(x => x.BuildingId == (int)Buildings.MetalMadeni);

                    #endregion

                    #region Kaynak Hesaplama

                    // Eğer kaynak binası yükseltiliyor ve yükseltme tamamlanmış ise yeni ve eski seviyedeki üretimleri ayrı ayrı hesaplamak gerkeiyor.
                    if (metalBuildingUpg != null && metalBuildingUpg.EndDate <= currentDate)
                    {
                        #region Kaynak binasının yükseltmesi tamamlandıktan sonraki hesaplama

                        // Yükseltmeden sonra geçen süreyi hesaplıyoruz.
                        double passedSecondsInNewLevel = (currentDate - metalBuildingUpg.EndDate).TotalSeconds;

                        // Yükseltmeden önceki geçen süreyi buluyoruz.
                        double passedSecondsInPrevLevels = passedSeconds - passedSecondsInNewLevel;

                        // Önceki seviyede toplam üretilen kaynak miktarını hesaplıyoruz.
                        double metalProduceQuantity = StaticData.GetBuildingProdPerHour(Buildings.MetalMadeni, metalBuilding == null ? 0 : metalBuilding.BuildingLevel) * (passedSecondsInPrevLevels / 3600);

                        // Yeni seviyede toplam üretilen kaynak miktarını hesaplıyoruz.
                        metalProduceQuantity += StaticData.GetBuildingProdPerHour(Buildings.MetalMadeni, metalBuildingUpg.BuildingLevel) * (passedSecondsInNewLevel / 3600);

                        // Kaynak depo kapasitesi.
                        long metalBuildingCapacity = (long)StaticData.GetBuildingStorage(Buildings.MetalDeposu, metalStorageBuilding == null ? 0 : metalStorageBuilding.BuildingLevel);

                        // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                        if (userPlanet.Metal < metalBuildingCapacity)
                        {
                            // Kaynakları depoya koyuyoruz.
                            userPlanet.Metal += (long)metalProduceQuantity;

                            // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                            if (userPlanet.Metal > metalBuildingCapacity)
                                userPlanet.Metal = metalBuildingCapacity;
                        }

                        // Yükseltmeyi siliyoruz.
                        uow.GetRepository<TblUserPlanetBuildingUpgs>().Delete(metalBuildingUpg);

                        // Kaynak binasının seviyesini güncelliyoruz.
                        metalBuilding.BuildingLevel = metalBuildingUpg.BuildingLevel;

                        #endregion
                    }
                    else // Eğer üretim binası yükseltimiyor yada henüz tamamlanmamış ise yükseltmeden önceki seviyeye göre hesaplama yapmamız yeterli.
                    {
                        // Metal deposu yükseltiliyor mu?
                        if (metalStorageBuildingUpg != null)
                        {
                            // Yükseltmenin bittiğinden emin oluyoruz.
                            if (metalStorageBuildingUpg.EndDate < currentDate)
                            {
                                #region Deponun yükseltilmesinden önceki hesaplama.

                                // Deponun yükseltmeden önceki geçen süre.
                                double passedSecondsInPrevStorage = (metalStorageBuildingUpg.EndDate - userPlanet.LastUpdateDate).TotalSeconds;

                                // Deponun yükseltmeden önceki kapasitesi.
                                long metalBuildingCapacityInPrevStorage = (long)StaticData.GetBuildingStorage(Buildings.MetalDeposu, metalStorageBuilding == null ? 0 : metalStorageBuilding.BuildingLevel);

                                // Metal binasının depo yükseltilene kadar ürettiği toplam miktar.
                                double metalProduceQuantityInPrevStorage = StaticData.GetBuildingProdPerHour(Buildings.MetalMadeni, metalBuilding == null ? 0 : metalBuilding.BuildingLevel) * (passedSecondsInPrevStorage / 3600);

                                // Üretilen toplam miktarı kontrol ediyoruz depo kapasitesinden fazla mı?
                                if (userPlanet.Metal < metalBuildingCapacityInPrevStorage)
                                {
                                    // Eğer değil ise kaynakları depoya ekliyoruz.
                                    userPlanet.Metal += (long)metalProduceQuantityInPrevStorage;

                                    // Eğer eklenen kaynaklar depoyu aştıysa fazlalığı atıyoruz.
                                    if (userPlanet.Metal > metalBuildingCapacityInPrevStorage)
                                        userPlanet.Metal = metalBuildingCapacityInPrevStorage;
                                }

                                #endregion

                                #region Depo yükseltme işlemi.

                                // Depoya yeni seviyeyi veriyoruz.
                                metalStorageBuilding.BuildingLevel = metalStorageBuildingUpg.BuildingLevel;

                                // Yükseltme bilgisini siliyoruz.
                                uow.GetRepository<TblUserPlanetBuildingUpgs>().Delete(metalStorageBuildingUpg);

                                #endregion

                                #region Deponun yükseltilmesinden sonraki hesaplama.

                                // Deponun yükseltmesinden sonraki geçen süre.
                                double passedSecondsInNextStorage = (currentDate - metalStorageBuildingUpg.EndDate).TotalSeconds;

                                // Deponun yükseltmeden sonraki kapasitesi.
                                long metalBuildingCapacityInNextStorage = (long)StaticData.GetBuildingStorage(Buildings.MetalDeposu, metalStorageBuilding == null ? 0 : metalStorageBuilding.BuildingLevel);

                                // Metal binasının yükseltmeden sonraki geçen sürede ürettiği metal miktarı.
                                double metalProduceQuantityInNextStorage = StaticData.GetBuildingProdPerHour(Buildings.MetalMadeni, metalBuilding == null ? 0 : metalBuilding.BuildingLevel) * (passedSecondsInNextStorage / 3600);

                                // Gezegendeki metal miktarını kontrol ediyoruz. Eğer depo sınırına ulaşmamış ise kaynakları depoya ekleyeceğiz.
                                if (userPlanet.Metal < metalBuildingCapacityInNextStorage)
                                {
                                    // Kaynağı depoya ekliyoruz.
                                    userPlanet.Metal += (long)metalProduceQuantityInNextStorage;

                                    // Yeni kaynak miktarı depo kapasitesinden büyük ise depo sınırına eşitliyoruz.
                                    if (userPlanet.Metal > metalBuildingCapacityInNextStorage)
                                        userPlanet.Metal = metalBuildingCapacityInNextStorage;
                                }

                                #endregion

                            }
                        }
                        else // Yükseltilmiyor ise standart ekleme çıkarma yapacağız.
                        {
                            #region Kaynak üretimi (Herhangi bir yükseltme olmadan.)

                            // Toplam geçen sürede kaynak binasının ürettiği toplam üretim.
                            double metalProduceQuantity = StaticData.GetBuildingProdPerHour(Buildings.MetalMadeni, metalBuilding == null ? 0 : metalBuilding.BuildingLevel) * (passedSeconds / 3600);

                            // Metal binasının kapasitesini hesaplıyoruz.
                            long metalBuildingCapacity = (long)StaticData.GetBuildingStorage(Buildings.MetalDeposu, metalStorageBuilding == null ? 0 : metalStorageBuilding.BuildingLevel);

                            // Depo da yer var ise depoya kaynağı ekliyoruz.
                            if (userPlanet.Metal < metalBuildingCapacity)
                            {
                                // Kaynağı depoya ekliyoruz.
                                userPlanet.Metal += (long)metalProduceQuantity;

                                // Eğer eklenen kaynak ile birlikte depo sınırına ulaşıldıysa fazlalığı siliyoruz.
                                if (userPlanet.Metal > metalBuildingCapacity)
                                    userPlanet.Metal = metalBuildingCapacity;
                            }

                            #endregion
                        }
                    }

                    #endregion
                }
                #endregion

                #region Kristal Üretimi
                {
                    // En son güncellemeden bu yana geçen süreyi buluyoruz.
                    double passedSeconds = (currentDate - userPlanet.LastUpdateDate).TotalSeconds;

                    // Kristal binasını buluyoruz.
                    TblUserPlanetBuildings crystalBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)Buildings.KristalMadeni);

                    // Üretilen miktar.
                    double crystalProduceQuantity = StaticData.GetBuildingProdPerHour(Buildings.KristalMadeni, crystalBuilding == null ? 0 : crystalBuilding.BuildingLevel) * (passedSeconds / 3600);

                    #region Metal Deposunu kontrol ediyoruz.

                    // Kristal deposunu buluyoruz.
                    TblUserPlanetBuildings crystalCapacityBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)Buildings.KristalDeposu);

                    // Kristal binası kapasitesi.
                    long crystalBuildingCapacity = (long)StaticData.GetBuildingStorage(Buildings.KristalDeposu, crystalCapacityBuilding == null ? 0 : crystalCapacityBuilding.BuildingLevel);

                    #endregion

                    // Üretilen kristali kullanıcıya veriyoruz ancak kapasitenin yeterli olması lazım.
                    if (userPlanet.Crystal < crystalBuildingCapacity)
                    {
                        // Üretim kristalini veriyoruz.
                        userPlanet.Crystal += (long)crystalProduceQuantity;

                        // Eğer kapasiteyi aştıysak kapasiteye ayarlıyoruz.
                        if (userPlanet.Crystal > crystalBuildingCapacity)
                            userPlanet.Crystal = crystalBuildingCapacity;
                    }
                }

                #endregion

                #region Boron Üretimi
                {
                    // En son güncellemeden bu yana geçen süreyi buluyoruz.
                    double passedSeconds = (currentDate - userPlanet.LastUpdateDate).TotalSeconds;

                    // Boron binasını buluyoruz.
                    TblUserPlanetBuildings boronBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)Buildings.BoronMadeni);

                    // Üretilen miktar.
                    double boronProduceQuantity = StaticData.GetBuildingProdPerHour(Buildings.BoronMadeni, boronBuilding == null ? 0 : boronBuilding.BuildingLevel) * (passedSeconds / 3600);

                    #region Boron Deposunu kontrol ediyoruz.

                    // Kristal deposunu buluyoruz.
                    TblUserPlanetBuildings boronCapacityBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)Buildings.BoronDeposu);

                    // Boron binası kapasitesi.
                    long boronBuildingCapacity = (long)StaticData.GetBuildingStorage(Buildings.BoronDeposu, boronCapacityBuilding == null ? 0 : boronCapacityBuilding.BuildingLevel);

                    #endregion

                    // Üretilen boron kullanıcıya veriyoruz ancak kapasitenin yeterli olması lazım.
                    if (userPlanet.Boron < boronBuildingCapacity)
                    {
                        // Üretim boron veriyoruz.
                        userPlanet.Boron += (long)boronProduceQuantity;

                        // Eğer kapasiteyi aştıysak kapasiteye ayarlıyoruz.
                        if (userPlanet.Boron > boronBuildingCapacity)
                            userPlanet.Boron = boronBuildingCapacity;
                    }
                }

                #endregion

                // Güncelleme tarihini değiştiriyoruz.
                userPlanet.LastUpdateDate = currentDate;

                // Değişiklikleri kayıt ediyoruz.
                uow.SaveChanges();

                // Gezegendeki üretimleri doğruladığımızı söylüyoruz.
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
            finally
            {
                // Listeden siliyoruz.
                lock (Verifies)
                    Verifies.Remove(verifyData.UserPlanetID);
            }
        }


    }
}