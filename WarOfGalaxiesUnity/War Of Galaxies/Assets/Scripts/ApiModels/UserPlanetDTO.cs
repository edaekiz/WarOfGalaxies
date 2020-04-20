using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class UserPlanetDTO
    {
        // Üretim binaları ve depoları. Önce kaynak sonra depo.
        public static Buildings[] ResouceBuildings = new Buildings[] {
                    Buildings.MetalMadeni,
                    Buildings.MetalDeposu,
                    Buildings.KristalMadeni,
                    Buildings.KristalDeposu,
                    Buildings.BoronMadeni,
                    Buildings.BoronDeposu
                };

        public int UserPlanetId;
        public int UserId;
        public string PlanetCordinate;
        public int PlanetType;
        public string PlanetName;
        public double Metal;
        public double Crystal;
        public double Boron;
        public DateTime LastUpdateDateInClient;
        public UserPlanetDTO()
        {
            LastUpdateDateInClient = DateTime.UtcNow;
        }

        /// <summary>
        /// Kaynak hesaplamalarını yapar.
        /// </summary>
        public void VerifyResources()
        {
            // Şuanki tarih.
            DateTime currentDate = DateTime.UtcNow;

            #region Üretim hesaplamaları.

            for (int ii = 0; ii < ResouceBuildings.Length; ii += 2)
            {
                // Kaynak binası.
                Buildings resourceBuilding = ResouceBuildings[ii];

                // Kaynak deposu.
                Buildings resourceStorageBuilding = ResouceBuildings[ii + 1];

                // En son güncellemeden bu yana geçen süreyi buluyoruz.
                double passedSeconds = (currentDate - this.LastUpdateDateInClient).TotalSeconds;

                #region Kaynak Binaları

                // Gezegendeki kaynak deposunu buluyoruz..
                UserPlanetBuildingDTO planetStorageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == this.UserPlanetId && x.BuildingId == resourceStorageBuilding);

                // Gezegendeki kaynak deposunun yükseltmesini buluyoruz.
                UserPlanetBuildingUpgDTO planetStorageBuildingUpg = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Find(x => x.UserPlanetId == this.UserPlanetId && x.BuildingId == resourceStorageBuilding);

                // Gezegendeki kaynak binasını buluyoruz.
                UserPlanetBuildingDTO planetResourceBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == this.UserPlanetId && x.BuildingId == resourceBuilding);

                // Gezegendeki kaynak binasının yükseltmesini buluyoruz.
                UserPlanetBuildingUpgDTO planetResourceBuildingUpg = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Find(x => x.UserPlanetId == this.UserPlanetId && x.BuildingId == resourceBuilding);

                #endregion

                #region Kaynak Hesaplama

                // Toplam geçen sürede kaynak binasının ürettiği toplam üretim.
                double metalProduceQuantity = StaticData.GetBuildingProdPerHour(resourceBuilding, planetResourceBuilding == null ? 0 : planetResourceBuilding.BuildingLevel) * (passedSeconds / 3600);

                // Metal binasının kapasitesini hesaplıyoruz.
                double metalBuildingCapacity = StaticData.GetBuildingStorage(resourceStorageBuilding, planetStorageBuilding == null ? 0 : planetStorageBuilding.BuildingLevel);

                // Gezegendeki kaynağı yükseltiyoruz.
                UpdateUserPlanetResources(resourceBuilding, metalBuildingCapacity, metalProduceQuantity);

                #endregion
            }

            // Güncelleme tarihini değiştiriyoruz.
            this.LastUpdateDateInClient = currentDate;

            #endregion
        }

        /// <summary>
        /// Verilen türe göre kaynakları günceller.
        /// </summary>
        /// <param name="userPlanet">Kullanıcının hangi gezegeninin kaynakları güncellenecek.</param>
        /// <param name="building">Hangi kaynak için yapılacak.</param>
        /// <param name="capacity">Kaynak depo kapasitesi.</param>
        /// <param name="quantity">Kaynak miktarı.</param>
        public void UpdateUserPlanetResources(Buildings building, double capacity, double quantity)
        {
            switch (building)
            {
                case Buildings.MetalMadeni:
                    // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                    if (this.Metal < capacity)
                    {
                        // Kaynakları depoya koyuyoruz.
                        this.Metal += quantity;

                        // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                        if (this.Metal > capacity)
                            this.Metal = capacity;
                    }
                    break;
                case Buildings.KristalMadeni:
                    // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                    if (this.Crystal < capacity)
                    {
                        // Kaynakları depoya koyuyoruz.
                        this.Crystal += quantity;

                        // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                        if (this.Crystal > capacity)
                            this.Crystal = capacity;
                    }
                    break;
                case Buildings.BoronMadeni:
                    // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                    if (this.Boron < capacity)
                    {
                        // Kaynakları depoya koyuyoruz.
                        this.Boron += quantity;

                        // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                        if (this.Boron > capacity)
                            this.Boron = capacity;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Kaynakları gezegene ekler.
        /// </summary>
        /// <param name="resources"></param>
        public void SetPlanetResources(ResourcesDTO resources)
        {
            // Yeni metal değerine set ediyoruz.
            this.Metal = resources.Metal;

            // Yeni kristal değerine set ediyoruz.
            this.Crystal = resources.Crystal;

            // Yeni boron değerine set ediyoruz.
            this.Boron = resources.Boron;

            // Son verify tarihini güncelliyoruz.
            this.LastUpdateDateInClient = DateTime.UtcNow;

        }

        /// <summary>
        /// Başka bir gezgenin değerlerini kopyalar.
        /// </summary>
        /// <param name="userPlanet"></param>
        public void CopyTo(UserPlanetDTO userPlanet)
        {
            userPlanet.Metal = this.Metal;
            userPlanet.Crystal = this.Crystal;
            userPlanet.Boron = this.Boron;
            userPlanet.PlanetCordinate = this.PlanetCordinate;
            userPlanet.PlanetName = this.PlanetName;
            userPlanet.PlanetType = this.PlanetType;
            userPlanet.LastUpdateDateInClient = DateTime.UtcNow;
            userPlanet.UserPlanetId = this.UserPlanetId;
        }

    }
}
