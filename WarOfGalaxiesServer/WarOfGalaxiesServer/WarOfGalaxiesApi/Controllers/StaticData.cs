using System;
using System.Collections.Generic;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Models;

namespace WarOfGalaxiesApi.Controllers
{
    public static class StaticData
    {

        /// <summary>
        /// Evrenin hızı.
        /// </summary>
        public static int UniverseSpeed = 10;

        /// <summary>
        /// Üretim binalarının listesi.
        /// 1-> Bina türü.
        /// 2-> Bina taban maliyeti.
        /// 3-> Bina taban değeri (Üretim/ Depolama vb).
        /// 4-> Binanın seviye başına maliyet artış oranı.
        /// </summary>
        public static List<Tuple<Buildings, ResourcesDTO, double, double>> ResourceBuildings = new List<Tuple<Buildings, ResourcesDTO, double, double>>()
        {
            new Tuple<Buildings, ResourcesDTO, double, double>(Buildings.MetalMadeni,new ResourcesDTO(60, 15, 0),90,1.5f),
            new Tuple<Buildings, ResourcesDTO, double, double>(Buildings.KristalMadeni,new ResourcesDTO(48, 24, 0),60,1.6f),
            new Tuple<Buildings, ResourcesDTO, double, double>(Buildings.BoronMadeni,new ResourcesDTO(225, 75, 0),30,1.5f),
            new Tuple<Buildings, ResourcesDTO, double, double>(Buildings.MetalDeposu, new ResourcesDTO(1000, 0, 0),75000,2),
            new Tuple<Buildings, ResourcesDTO, double, double>(Buildings.KristalDeposu,new ResourcesDTO(1000, 500, 0),50000,2),
            new Tuple<Buildings, ResourcesDTO, double, double>(Buildings.BoronDeposu,new ResourcesDTO(1000, 1000, 0),25000,2),
            new Tuple<Buildings, ResourcesDTO, double, double>(Buildings.RobotFabrikası,new ResourcesDTO(400,120,200),0,2),
            new Tuple<Buildings, ResourcesDTO, double, double>(Buildings.ArastirmaLab,new ResourcesDTO(200,400,200),0,2),
            new Tuple<Buildings, ResourcesDTO, double, double>(Buildings.Tersane,new ResourcesDTO(400,200,100),0,2),
        };

        /// <summary>
        /// Binanın seviye bazlı maliyetini hesaplıyoruz.
        /// </summary>
        /// <param name="building"></param>
        /// <param name="buildingLevel"></param>
        public static ResourcesDTO CalculateCostBuilding(Buildings building, int buildingLevel)
        {
            // Kaynağın temel maliyeti.
            Tuple<Buildings, ResourcesDTO, double, double> baseCost = ResourceBuildings.Find(x => x.Item1 == building);

            // Hesaplayıp geri dönüyoruz.
            return baseCost.Item2 * Math.Pow(baseCost.Item4, buildingLevel);
        }

        /// <summary>
        /// Binanın yükseltme süresini hesaplar. Saniye cinsinden
        /// </summary>
        /// <param name="building">Süresi hesaplanacak bina.</param>
        /// <param name="robotFactoryLevel">Robot fabrikası seviyesi.</param>
        /// <returns></returns>
        public static double CalculateBuildingUpgradeTime(Buildings building, int buildingLevel, int robotFactoryLevel)
        {
            ResourcesDTO buildingCost = CalculateCostBuilding(building, buildingLevel);
            return ((buildingCost.Metal + buildingCost.Crystal) / ((double)2500 * (1 + robotFactoryLevel) * UniverseSpeed)) * 3600;
        }

        /// <summary>
        /// Binaların saatlik üretimini hesaplıyoruz.
        /// </summary>
        /// <param name="building">Hangi bina için hesaplanacak. (Metal/Kristal/Boron)</param>
        /// <param name="buildingLevel">Binanı seviyesi.</param>
        /// <returns></returns>
        public static double GetBuildingProdPerHour(Buildings building, int buildingLevel)
        {
            Tuple<Buildings, ResourcesDTO, double, double> buildingInfo = ResourceBuildings.Find(x => x.Item1 == building);
            return buildingInfo.Item3 * UniverseSpeed * buildingLevel * Math.Pow(1.1f, buildingLevel) + buildingInfo.Item3 * UniverseSpeed;
        }

        /// <summary>
        /// Binanın depo kapasitesi.
        /// </summary>
        /// <param name="buildingLevel">Binanın seviyesi.</param>
        /// <returns></returns>
        public static double GetBuildingStorage(Buildings building, int buildingLevel)
        {
            Tuple<Buildings, ResourcesDTO, double, double> buildingInfo = ResourceBuildings.Find(x => x.Item1 == building);
            return buildingInfo.Item3 + 50000 * (Math.Pow(1.6f, buildingLevel) - 1);
        }

        #region Researches / Araştırmalar

        /// <summary>
        /// Araştırmalar ve gereksinimleri ve oranları.
        /// İlk parametre Araştırma.
        /// İkinci Parametre Araştırma taban maliyeti.
        /// Üçüncü Araştırma değeri.
        /// </summary>
        public static List<Tuple<Researches, ResourcesDTO, double>> ResearchData = new List<Tuple<Researches, ResourcesDTO, double>>()
        {
            new Tuple<Researches, ResourcesDTO, double>(Researches.Casusluk,new ResourcesDTO(100,200,100),.1f),
            new Tuple<Researches, ResourcesDTO, double>(Researches.FiloYönetimi,new ResourcesDTO(0,400,200),1),
            new Tuple<Researches, ResourcesDTO, double>(Researches.KalkanTekniği,new ResourcesDTO(120,30),.1f),
            new Tuple<Researches, ResourcesDTO, double>(Researches.Kaşiflik,new ResourcesDTO(400,200,100),.1f),
            new Tuple<Researches, ResourcesDTO, double>(Researches.LazerSilahları,new ResourcesDTO(200,100),.1f),
            new Tuple<Researches, ResourcesDTO, double>(Researches.MotorTekniği,new ResourcesDTO(1600,800,400),.1f),
            new Tuple<Researches, ResourcesDTO, double>(Researches.UzayGemisiZırhı,new ResourcesDTO(400,200),.1f),
            new Tuple<Researches, ResourcesDTO, double>(Researches.YükKapasitesi,new ResourcesDTO(1200,600,600),.05f),
            new Tuple<Researches, ResourcesDTO, double>(Researches.İticiler,new ResourcesDTO(200,800,600),.3f),
            new Tuple<Researches, ResourcesDTO, double>(Researches.Silahlandırma,new ResourcesDTO(200,100),.1f)
        };

        /// <summary>
        /// Araştırmaların maliyetini verilen seviye için hesaplar.
        /// </summary>
        /// <param name="research">Hangi araştırma için hesaplanacak.</param>
        /// <param name="researchLevel">Araştırmanın seviyesi.</param>
        /// <returns></returns>
        public static ResourcesDTO CalculateCostResearch(Researches research, int researchLevel)
        {
            // Araştırma ve datalarını buluyoruz.
            Tuple<Researches, ResourcesDTO, double> researchItem = ResearchData.Find(x => x.Item1 == research);

            // Hesaplayıp geri dönüyoruz. Zaten her seviye için kaynakları seviye ile çarpıyoruz.
            return new ResourcesDTO(Math.Pow(2, researchLevel) * researchItem.Item2.Metal, Math.Pow(2, researchLevel) * researchItem.Item2.Crystal, Math.Pow(2, researchLevel) * researchItem.Item2.Boron);
        }

        /// <summary>
        /// Araştırmaların yükseltme süresini hesaplar.
        /// </summary>
        /// <param name="research">Hangi araştırma için hesaplanacak.</param>
        /// <param name="researchLevel">Araştırmanın seviyesi.</param>
        /// <returns></returns>
        public static double CalculateResearchUpgradeTime(Researches research, int researchLevel)
        {
            // Maliyetini hesaplıyoruz araştırmanın.
            ResourcesDTO cost = CalculateCostResearch(research, researchLevel);

            // Metal ve kristal üzerinden araştırm süresini hesaplıyoruz.
            return ((cost.Metal + cost.Crystal) / (UniverseSpeed * 1000 * (1 + researchLevel))) * 3600;
        }

        #endregion

        #region Shipyard Ships

        public static List<ShipDTO> ShipData = new List<ShipDTO>()
        {
            new ShipDTO
            {
                Cost = new ResourcesDTO(1500,750),
                ShipID = Ships.HafifAvci,
                ShipSpeed = 10000
            }
        };

        /// <summary>
        /// Gemilerin üretim süresini hesaplar.
        /// </summary>
        /// <param name="ship">Hesaplanacak gemi.</param>
        /// <param name="shipyardLevel">Hesaplanacak liman seviyesi.</param>
        /// <returns></returns>
        public static double CalculateShipCountdown(Ships ship, int shipyardLevel)
        {
            ShipDTO shipInfo = ShipData.Find(x => x.ShipID == ship);
            return ((shipInfo.Cost.Metal + shipInfo.Cost.Crystal) / (2500 * (10 + shipyardLevel))) * 3600;
        }

        #endregion

        #region Defenses

        public static List<DefenseDTO> DefenseData = new List<DefenseDTO>()
        {
            new DefenseDTO
            {
                Cost = new ResourcesDTO(500,0),
                DefenseID = Defenses.Roketatar
            }
        };

        /// <summary>
        /// Defans üretim süresini hesaplar.
        /// </summary>
        /// <param name="defense">Hesaplanacak savunma.</param>
        /// <param name="robotLevel">Hesaplanacak robot fabrikası seviyesi.</param>
        /// <returns></returns>
        public static double CalculateDefenseCountdown(Defenses defense, int robotLevel)
        {
            DefenseDTO defenseInfo = DefenseData.Find(x => x.DefenseID == defense);
            return ((defenseInfo.Cost.Metal + defenseInfo.Cost.Crystal) / (2500 * (10 + robotLevel))) * 3600;
        }

        #endregion

    }
}
