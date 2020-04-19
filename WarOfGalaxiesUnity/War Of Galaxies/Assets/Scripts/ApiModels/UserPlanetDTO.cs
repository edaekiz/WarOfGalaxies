using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class UserPlanetDTO
    {
        public int UserPlanetId;
        public int UserId;
        public string PlanetCordinate;
        public int PlanetType;
        public string PlanetName;
        public long Metal;
        public long Crystal;
        public long Boron;
        public DateTime LastUpdateDateInClient;
        public UserPlanetDTO()
        {
            LastUpdateDateInClient = DateTime.UtcNow;
        }

        public void VerifyResources()
        {
            // Şuanki tarih.
            DateTime currentDate = DateTime.UtcNow;

            // Son güncellemesinden bu yana geçen süre.
            double passedSeconds = (currentDate - this.LastUpdateDateInClient).TotalSeconds;

            // Güncelleme tarihini değiştiriyoruz.
            this.LastUpdateDateInClient = currentDate;

            #region Metal Üretimi

            {
                // Metal binasını buluyoruz.
                UserPlanetBuildingDTO metalBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == this.UserPlanetId && x.BuildingId == Buildings.MetalMadeni);

                // Üretilen miktar.
                double metalProduceQuantity = StaticData.GetBuildingProdPerHour(Buildings.MetalMadeni, metalBuilding == null ? 0 : metalBuilding.BuildingLevel) * (passedSeconds / 3600);

                #region Metal Deposunu kontrol ediyoruz.

                // Metal deposunu buluyoruz.
                UserPlanetBuildingDTO metalCapacityBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == this.UserPlanetId && x.BuildingId == Buildings.MetalDeposu);

                // Metal binası kapasitesi.
                long metalBuildingCapacity = (long)StaticData.GetBuildingStorage(Buildings.MetalDeposu, metalCapacityBuilding == null ? 0 : metalCapacityBuilding.BuildingLevel);

                #endregion

                // Üretilen metali kullanıcıya veriyoruz ancak kapasitenin yeterli olması lazım.
                if (this.Metal < metalBuildingCapacity)
                {
                    // Üretim metalini veriyoruz.
                    this.Metal += (long)metalProduceQuantity;

                    // Eğer kapasiteyi aştıysak kapasiteye ayarlıyoruz.
                    if (this.Metal > metalBuildingCapacity)
                        this.Metal = metalBuildingCapacity;
                }
            }

            #endregion

            #region Kristal Üretimi
            {

                // Kristal binasını buluyoruz.
                UserPlanetBuildingDTO crystalBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == this.UserPlanetId && x.BuildingId == Buildings.KristalMadeni);

                // Üretilen miktar.
                double crystalProduceQuantity = StaticData.GetBuildingProdPerHour(Buildings.KristalMadeni, crystalBuilding == null ? 0 : crystalBuilding.BuildingLevel) * (passedSeconds / 3600);

                #region Metal Deposunu kontrol ediyoruz.

                // Kristal deposunu buluyoruz.
                UserPlanetBuildingDTO crystalCapacityBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == this.UserPlanetId && x.BuildingId == Buildings.KristalDeposu);

                // Kristal binası kapasitesi.
                long crystalBuildingCapacity = (long)StaticData.GetBuildingStorage(Buildings.KristalDeposu, crystalCapacityBuilding == null ? 0 : crystalCapacityBuilding.BuildingLevel);

                #endregion

                // Üretilen kristali kullanıcıya veriyoruz ancak kapasitenin yeterli olması lazım.
                if (this.Crystal < crystalBuildingCapacity)
                {
                    // Üretim kristalini veriyoruz.
                    this.Crystal += (long)crystalProduceQuantity;

                    // Eğer kapasiteyi aştıysak kapasiteye ayarlıyoruz.
                    if (this.Crystal > crystalBuildingCapacity)
                        this.Crystal = crystalBuildingCapacity;
                }
            }

            #endregion
        }

    }
}
