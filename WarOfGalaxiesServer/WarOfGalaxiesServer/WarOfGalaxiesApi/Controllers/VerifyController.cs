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
        ///  Üretim binaları ve depoları. Önce kaynak sonra depo.
        /// </summary>
        public static Buildings[] ResouceBuildings = new Buildings[] {
                    Buildings.MetalMadeni,
                    Buildings.MetalDeposu,
                    Buildings.KristalMadeni,
                    Buildings.KristalDeposu,
                    Buildings.BoronMadeni,
                    Buildings.BoronDeposu
                };

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

                #region Üretim hesaplamaları.

                for (int ii = 0; ii < ResouceBuildings.Length; ii += 2)
                {
                    // Kaynak binası.
                    Buildings resourceBuilding = ResouceBuildings[ii];

                    // Kaynak deposu.
                    Buildings resourceStorageBuilding = ResouceBuildings[ii + 1];

                    // En son güncellemeden bu yana geçen süreyi buluyoruz.
                    double passedSeconds = (currentDate - userPlanet.LastUpdateDate).TotalSeconds;

                    #region Kaynak Binaları

                    // Gezegendeki kaynak deposunu buluyoruz..
                    TblUserPlanetBuildings planetStorageBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)resourceStorageBuilding);

                    // Gezegendeki kaynak deposunun yükseltmesini buluyoruz.
                    TblUserPlanetBuildingUpgs planetStorageBuildingUpg = userPlanetBuildingUpgs.Find(x => x.BuildingId == (int)resourceStorageBuilding);

                    // Gezegendeki kaynak binasını buluyoruz.
                    TblUserPlanetBuildings planetResourceBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)resourceBuilding);

                    // Gezegendeki kaynak binasının yükseltmesini buluyoruz.
                    TblUserPlanetBuildingUpgs planetResourceBuildingUpg = userPlanetBuildingUpgs.Find(x => x.BuildingId == (int)resourceBuilding);

                    #endregion

                    #region Kaynak Hesaplama

                    // Eğer kaynak binası yükseltiliyor ve yükseltme tamamlanmış ise yeni ve eski seviyedeki üretimleri ayrı ayrı hesaplamak gerkeiyor.
                    if (planetResourceBuildingUpg != null && planetResourceBuildingUpg.EndDate <= currentDate)
                    {
                        #region Kaynak binasının yükseltmesi tamamlandıktan sonraki hesaplama

                        // Yükseltmeden sonra geçen süreyi hesaplıyoruz.
                        double passedSecondsInNewLevel = (currentDate - planetResourceBuildingUpg.EndDate).TotalSeconds;

                        // Yükseltmeden önceki geçen süreyi buluyoruz.
                        double passedSecondsInPrevLevels = passedSeconds - passedSecondsInNewLevel;

                        // Önceki seviyede toplam üretilen kaynak miktarını hesaplıyoruz.
                        double metalProduceQuantity = StaticData.GetBuildingProdPerHour(resourceBuilding, planetResourceBuilding == null ? 0 : planetResourceBuilding.BuildingLevel) * (passedSecondsInPrevLevels / 3600);

                        // Yeni seviyede toplam üretilen kaynak miktarını hesaplıyoruz.
                        metalProduceQuantity += StaticData.GetBuildingProdPerHour(resourceBuilding, planetResourceBuildingUpg.BuildingLevel) * (passedSecondsInNewLevel / 3600);

                        // Kaynak depo kapasitesi.
                        double metalBuildingCapacity = StaticData.GetBuildingStorage(resourceStorageBuilding, planetStorageBuilding == null ? 0 : planetStorageBuilding.BuildingLevel);

                        // Gezegendeki kaynağı yükseltiyoruz.
                        UpdateUserPlanetResources(userPlanet, resourceBuilding, metalBuildingCapacity, metalProduceQuantity);

                        // Yükseltmeyi siliyoruz.
                        uow.GetRepository<TblUserPlanetBuildingUpgs>().Delete(planetResourceBuildingUpg);

                        // Eğer bina yok ise ozaman binayı oluşturacağız.
                        if (planetResourceBuilding == null)
                        {
                            // Ve binayı sisteme ekliyoruz.
                            uow.GetRepository<TblUserPlanetBuildings>().Add(new TblUserPlanetBuildings
                            {
                                BuildingLevel = 1,
                                BuildingId = (int)resourceBuilding,
                                UserId = userPlanet.UserId,
                                UserPlanetId = userPlanet.UserPlanetId
                            });
                        }
                        else // Zaten kaynak binası var ise seviyesini yükseltiyoruz.
                            planetResourceBuilding.BuildingLevel = planetResourceBuildingUpg.BuildingLevel;

                        #endregion
                    }
                    else // Eğer üretim binası yükseltimiyor yada henüz tamamlanmamış ise yükseltmeden önceki seviyeye göre hesaplama yapmamız yeterli.
                    {
                        // Metal deposu yükseltiliyor mu?
                        if (planetStorageBuildingUpg != null)
                        {
                            // Yükseltmenin bittiğinden emin oluyoruz.
                            if (planetStorageBuildingUpg.EndDate < currentDate)
                            {
                                #region Deponun yükseltilmesinden önceki hesaplama.

                                // Deponun yükseltmeden önceki geçen süre.
                                double passedSecondsInPrevStorage = (planetStorageBuildingUpg.EndDate - userPlanet.LastUpdateDate).TotalSeconds;

                                // Deponun yükseltmeden önceki kapasitesi.
                                double metalBuildingCapacityInPrevStorage = StaticData.GetBuildingStorage(resourceStorageBuilding, planetStorageBuilding == null ? 0 : planetStorageBuilding.BuildingLevel);

                                // Metal binasının depo yükseltilene kadar ürettiği toplam miktar.
                                double metalProduceQuantityInPrevStorage = StaticData.GetBuildingProdPerHour(resourceBuilding, planetResourceBuilding == null ? 0 : planetResourceBuilding.BuildingLevel) * (passedSecondsInPrevStorage / 3600);

                                // Gezegendeki kaynağı yükseltiyoruz.
                                UpdateUserPlanetResources(userPlanet, resourceBuilding, metalBuildingCapacityInPrevStorage, metalProduceQuantityInPrevStorage);

                                #endregion

                                #region Depo yükseltme işlemi.

                                // Yükseltme bilgisini siliyoruz.
                                uow.GetRepository<TblUserPlanetBuildingUpgs>().Delete(planetStorageBuildingUpg);

                                // Metal deposunu inşaa etmemiz gerekiyor ise inşaa edeceğiz.
                                if (planetStorageBuilding == null)
                                {
                                    // Ve binayı sisteme ekliyoruz.
                                    uow.GetRepository<TblUserPlanetBuildings>().Add(new TblUserPlanetBuildings
                                    {
                                        BuildingLevel = 1,
                                        BuildingId = (int)resourceStorageBuilding,
                                        UserId = userPlanet.UserId,
                                        UserPlanetId = userPlanet.UserPlanetId
                                    });
                                }
                                else // Zaten kaynak deposu var ise seviyesini yükseltiyoruz.
                                    planetStorageBuilding.BuildingLevel = planetStorageBuildingUpg.BuildingLevel;

                                #endregion

                                #region Deponun yükseltilmesinden sonraki hesaplama.

                                // Deponun yükseltmesinden sonraki geçen süre.
                                double passedSecondsInNextStorage = (currentDate - planetStorageBuildingUpg.EndDate).TotalSeconds;

                                // Deponun yükseltmeden sonraki kapasitesi.
                                double metalBuildingCapacityInNextStorage = StaticData.GetBuildingStorage(resourceStorageBuilding, planetStorageBuilding == null ? 0 : planetStorageBuilding.BuildingLevel);

                                // Metal binasının yükseltmeden sonraki geçen sürede ürettiği metal miktarı.
                                double metalProduceQuantityInNextStorage = StaticData.GetBuildingProdPerHour(resourceBuilding, planetResourceBuilding == null ? 0 : planetResourceBuilding.BuildingLevel) * (passedSecondsInNextStorage / 3600);

                                // Gezegendeki kaynağı yükseltiyoruz.
                                UpdateUserPlanetResources(userPlanet, resourceBuilding, metalBuildingCapacityInNextStorage,metalProduceQuantityInNextStorage);

                                #endregion

                            }
                        }
                        else // Yükseltilmiyor ise standart ekleme çıkarma yapacağız.
                        {
                            #region Kaynak üretimi (Herhangi bir yükseltme olmadan.)

                            // Toplam geçen sürede kaynak binasının ürettiği toplam üretim.
                            double metalProduceQuantity = StaticData.GetBuildingProdPerHour(resourceBuilding, planetResourceBuilding == null ? 0 : planetResourceBuilding.BuildingLevel) * (passedSeconds / 3600);

                            // Metal binasının kapasitesini hesaplıyoruz.
                            double metalBuildingCapacity = StaticData.GetBuildingStorage(resourceStorageBuilding, planetStorageBuilding == null ? 0 : planetStorageBuilding.BuildingLevel);

                            // Gezegendeki kaynağı yükseltiyoruz.
                            UpdateUserPlanetResources(userPlanet, resourceBuilding, metalBuildingCapacity, metalProduceQuantity);

                            #endregion
                        }
                    }

                    #endregion
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

        /// <summary>
        /// Verilen türe göre kaynakları günceller.
        /// </summary>
        /// <param name="userPlanet">Kullanıcının hangi gezegeninin kaynakları güncellenecek.</param>
        /// <param name="building">Hangi kaynak için yapılacak.</param>
        /// <param name="capacity">Kaynak depo kapasitesi.</param>
        /// <param name="quantity">Kaynak miktarı.</param>
        public static void UpdateUserPlanetResources(TblUserPlanets userPlanet, Buildings building, double capacity, double quantity)
        {
            switch (building)
            {
                case Buildings.MetalMadeni:

                    // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                    if (userPlanet.Metal < capacity)
                    {
                        // Kaynakları depoya koyuyoruz.
                        userPlanet.Metal += quantity;

                        // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                        if (userPlanet.Metal > capacity)
                            userPlanet.Metal = capacity;
                    }

                    break;
                case Buildings.KristalMadeni:

                    // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                    if (userPlanet.Crystal < capacity)
                    {
                        // Kaynakları depoya koyuyoruz.
                        userPlanet.Crystal += quantity;

                        // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                        if (userPlanet.Crystal > capacity)
                            userPlanet.Crystal = capacity;
                    }

                    break;
                case Buildings.BoronMadeni:

                    // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                    if (userPlanet.Boron < capacity)
                    {
                        // Kaynakları depoya koyuyoruz.
                        userPlanet.Boron += quantity;

                        // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                        if (userPlanet.Boron > capacity)
                            userPlanet.Boron = capacity;
                    }

                    break;
                default:
                    break;
            }
        }

    }
}