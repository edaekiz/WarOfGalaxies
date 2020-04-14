using System;
using System.Collections.Generic;
using System.Linq;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DAL.Repositories;
using WarOfGalaxiesApi.DTO.ApiModels;
using WarOfGalaxiesApi.DTO.Enums;

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

                // Bütün seviye bilgilerini getirir.
                List<TblBuildingLevels> buildingLevels = uow.GetRepository<TblBuildingLevels>().ToList();

                // Gezegeni buluyoruz.
                TblUserPlanets userPlanet = uow.GetRepository<TblUserPlanets>().FirstOrDefault(x => x.UserPlanetId == verifyData.UserPlanetID);

                // Son güncellemesinden bu yana geçen süre.
                double passedSeconds = (currentDate - userPlanet.LastUpdateDate).TotalSeconds;

                // Kullanıcının gezegenindeki binaları alıyoruz.
                List<TblUserPlanetBuildings> userPlanetBuildings = uow.GetRepository<TblUserPlanetBuildings>().Where(x => x.UserPlanetId == verifyData.UserPlanetID).ToList();

                // Güncelleme tarihini değiştiriyoruz.
                userPlanet.LastUpdateDate = currentDate;

                #region Metal Üretimi

                // Metal binasını buluyoruz.
                TblUserPlanetBuildings metalBuilding = userPlanetBuildings.Find(x => x.BuildingId == (int)Buildings.MetalMadeni);

                // Metal binası var ise hesaplıyoruz.
                if (metalBuilding != null)
                {
                    // Metal binasının seviyesi.
                    TblBuildingLevels metalBuildingLevel = buildingLevels.Find(x => x.BuildingId == (int)Buildings.MetalMadeni && x.BuildingLevel == metalBuilding.BuildingLevel);

                    // Yükseltme bilgisini buluyoruz.
                    if (metalBuildingLevel != null)
                    {
                        // Üretilen miktar.
                        double metalProduceQuantity = metalBuildingLevel.BuildingValue * (passedSeconds / 3600);

                        // Üretilen metali kullanıcıya veriyoruz.
                        userPlanet.Metal += (long)metalProduceQuantity;

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