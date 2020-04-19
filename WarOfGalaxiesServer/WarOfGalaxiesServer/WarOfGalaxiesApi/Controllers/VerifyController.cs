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
                // Şuanki tarih.
                DateTime currentDate = DateTime.UtcNow;

                // Gezegeni buluyoruz.
                TblUserPlanets userPlanet = uow.GetRepository<TblUserPlanets>().FirstOrDefault(x => x.UserPlanetId == verifyData.UserPlanetID);

                // Son güncellemesinden bu yana geçen süre.
                double passedSeconds = (currentDate - userPlanet.LastUpdateDate).TotalSeconds;

                // Kullanıcının gezegenindeki binaları alıyoruz.
                List<TblUserPlanetBuildings> userPlanetBuildings = uow.GetRepository<TblUserPlanetBuildings>().Where(x => x.UserPlanetId == verifyData.UserPlanetID).ToList();

                // Güncelleme tarihini değiştiriyoruz.
                userPlanet.LastUpdateDate = currentDate;

                #region Metal Üretimi
                {
                    // Metal binasını buluyoruz.
                    TblUserPlanetBuildings metalBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)Buildings.MetalMadeni);

                    // Metal binası var ise hesaplıyoruz.

                    // Üretilen miktar.
                    double metalProduceQuantity = StaticData.GetBuildingProdPerHour(Buildings.MetalMadeni, metalBuilding == null ? 0 : metalBuilding.BuildingLevel) * (passedSeconds / 3600);

                    // Metal binasını buluyoruz.
                    TblUserPlanetBuildings metalCapacityBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)Buildings.MetalDeposu);

                    // Metal binası kapasitesi.
                    long metalBuildingCapacity = (long)StaticData.GetBuildingStorage(Buildings.MetalDeposu, metalCapacityBuilding == null ? 0 : metalCapacityBuilding.BuildingLevel);

                    // Üretilen metali kullanıcıya veriyoruz ancak kapasitenin yeterli olması lazım.
                    if (userPlanet.Metal < metalBuildingCapacity)
                    {
                        // Üretim metalini veriyoruz.
                        userPlanet.Metal += (long)metalProduceQuantity;

                        // Eğer kapasiteyi aştıysak kapasiteye ayarlıyoruz.
                        if (userPlanet.Metal > metalBuildingCapacity)
                            userPlanet.Metal = metalBuildingCapacity;
                    }
                }
                #endregion

                #region Kristal Üretimi
                {

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