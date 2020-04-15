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
        public DateTime LastUpdateDate;
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

            // Kullanıcının gezegenindeki binaları alıyoruz.
            List<UserPlanetBuildingDTO> userPlanetBuildings = GlobalBuildingController.GBC.UserPlanetBuildings.Where(x => x.UserPlanetId == this.UserPlanetId).ToList();

            // Güncelleme tarihini değiştiriyoruz.
            this.LastUpdateDateInClient = currentDate;

            #region Metal Üretimi

            // Metal binasını buluyoruz.
            UserPlanetBuildingDTO metalBuilding = userPlanetBuildings.Find(x => x.BuildingId == Buildings.MetalMadeni);

            // Metal binası var ise hesaplıyoruz.
            if (metalBuilding != null)
            {
                // Metal binasının seviyesi.
                BuildingLevelDTO metalBuildingLevel = GlobalBuildingController.GBC.BuildingLevels.Find(x => x.BuildingId == Buildings.MetalMadeni && x.BuildingLevel == metalBuilding.BuildingLevel);

                // Yükseltme bilgisini buluyoruz.
                if (metalBuildingLevel != null)
                {
                    // Üretilen miktar.
                    double metalProduceQuantity = metalBuildingLevel.BuildingValue * (passedSeconds / 3600);

                    // Toplam saklanabilir depo kapasitesi.
                    long metalCapacity = 0;

                    #region Metal Deposunu kontrol ediyoruz.

                    // Metal binasını buluyoruz.
                    UserPlanetBuildingDTO metalCapacityBuilding = userPlanetBuildings.Find(x => x.BuildingId == Buildings.MetalDeposu);

                    // Metal binası var ise hesaplıyoruz.
                    if (metalCapacityBuilding != null)
                    {
                        // Metal depo binasının seviyesi.
                        BuildingLevelDTO metalCapcityBuildingLevel = GlobalBuildingController.GBC.BuildingLevels.Find(x => x.BuildingId == Buildings.MetalDeposu && x.BuildingLevel == metalCapacityBuilding.BuildingLevel);

                        // Yükseltme bilgisini buluyoruz.
                        if (metalCapcityBuildingLevel != null)
                            metalCapacity += metalCapcityBuildingLevel.BuildingValue;
                    }

                    #endregion

                    // Üretilen metali kullanıcıya veriyoruz ancak kapasitenin yeterli olması lazım.
                    if (this.Metal < metalCapacity)
                    {
                        // Üretim metalini veriyoruz.
                        this.Metal += (long)metalProduceQuantity;

                        // Eğer kapasiteyi aştıysak kapasiteye ayarlıyoruz.
                        if (this.Metal > metalCapacity)
                            this.Metal = metalCapacity;
                    }

                }
            }

            #endregion
        }

    }
}
