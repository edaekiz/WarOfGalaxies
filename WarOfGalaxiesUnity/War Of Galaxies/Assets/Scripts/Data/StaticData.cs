using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;

namespace Assets.Scripts.Data
{
    public static class StaticData
    {

        /// <summary>
        /// Evrenin hızı.
        /// </summary>
        public static int UniverseSpeed = 1;

        /// <summary>
        /// Metal deposunun base kapasitesi.
        /// </summary>
        public static double MetalBuildingStorageCapacity = 100000;

        /// <summary>
        /// Kristal deposunun base kapasitesi.
        /// </summary>
        public static double CrystalBuildingStorageCapacity = 100000;

        /// <summary>
        /// Boron deposunun base kapasitesi.
        /// </summary>
        public static double BoronBuildingStorageCapacity = 100000;

        /// <summary>
        /// Metal madeninin basee saatlik üretimi
        /// </summary>
        public static double MetalBuildingBasePerHour = 30;

        /// <summary>
        /// Kristal madeninin base saatlik üretimi
        /// </summary>
        public static double CrystalBuildingBasePerHour = 20;

        /// <summary>
        /// Boron madeninin base saatlik üretimi
        /// </summary>
        public static double BoronBuildingBasePerHour = 10;

        /// <summary>
        /// Metal binasının taban gereksinimleri.
        /// </summary>
        public static ResourcesDTO MetalBuildingBaseCost = new ResourcesDTO(60, 15, 0);

        /// <summary>
        /// Kristal binasının taban gereksinimleri.
        /// </summary>
        public static ResourcesDTO CrystalBuildingBaseCost = new ResourcesDTO(48, 24, 0);

        /// <summary>
        /// Boron binasının taban gereksinimleri.
        /// </summary>
        public static ResourcesDTO BoronBuildingBaseCost = new ResourcesDTO(225, 75, 0);

        /// <summary>
        /// Metal binasının temel yükseltme değeri.
        /// </summary>
        public static ResourcesDTO MetalBuildingStorageBaseCost = new ResourcesDTO(1000, 0, 0);

        /// <summary>
        /// Kristal binasının temel yükseltme değeri.
        /// </summary>
        public static ResourcesDTO CrystalBuildingStorageBaseCost = new ResourcesDTO(1000, 500, 0);

        /// <summary>
        /// Boron binasının temel yükseltme değeri.
        /// </summary>
        public static ResourcesDTO BoronBuildingStorageBaseCost = new ResourcesDTO(1000, 1000, 0);

        /// <summary>
        /// Robot fabrikası temel yükseltme değeri.
        /// </summary>
        public static ResourcesDTO RobotFabrikasiBaseCost = new ResourcesDTO(400, 120, 200);

        /// <summary>
        /// Araştırma binası temel yükseltme değeri.
        /// </summary>
        public static ResourcesDTO ArastirmaLabBaseCost = new ResourcesDTO(200, 400, 200);

        /// <summary>
        /// Tersane binası temel yükseltme değeri.
        /// </summary>
        public static ResourcesDTO TersaneBaseCost = new ResourcesDTO(400, 200, 100);

        /// <summary>
        /// Binanın seviye bazlı maliyetini hesaplıyoruz.
        /// </summary>
        /// <param name="building"></param>
        /// <param name="buildingLevel"></param>
        public static ResourcesDTO CalculateCostBuilding(Buildings building, int buildingLevel)
        {
            switch (building)
            {
                case Buildings.MetalMadeni:
                    return new ResourcesDTO(
                        MetalBuildingBaseCost.Metal * Math.Pow(1.5f, buildingLevel),
                        MetalBuildingBaseCost.Crystal * Math.Pow(1.5f, buildingLevel),
                        MetalBuildingBaseCost.Boron * Math.Pow(1.5f, buildingLevel));
                case Buildings.KristalMadeni:
                    return new ResourcesDTO(
                        CrystalBuildingBaseCost.Metal * Math.Pow(1.6f, buildingLevel),
                        CrystalBuildingBaseCost.Crystal * Math.Pow(1.6f, buildingLevel),
                        CrystalBuildingBaseCost.Boron * Math.Pow(1.6f, buildingLevel));
                case Buildings.BoronMadeni:
                    return new ResourcesDTO(
                        BoronBuildingBaseCost.Metal * Math.Pow(1.5f, buildingLevel),
                        BoronBuildingBaseCost.Crystal * Math.Pow(1.5f, buildingLevel),
                        BoronBuildingBaseCost.Boron * Math.Pow(1.5f, buildingLevel));
                case Buildings.MetalDeposu:
                    return new ResourcesDTO(
                        MetalBuildingStorageBaseCost.Metal * Math.Pow(2, buildingLevel),
                        MetalBuildingStorageBaseCost.Crystal * Math.Pow(2, buildingLevel),
                        MetalBuildingStorageBaseCost.Boron * Math.Pow(2, buildingLevel));
                case Buildings.KristalDeposu:
                    return new ResourcesDTO(
                        CrystalBuildingStorageBaseCost.Metal * Math.Pow(2, buildingLevel),
                        CrystalBuildingStorageBaseCost.Crystal * Math.Pow(2, buildingLevel),
                        CrystalBuildingStorageBaseCost.Boron * Math.Pow(2, buildingLevel));
                case Buildings.BoronDeposu:
                    return new ResourcesDTO(
                        BoronBuildingStorageBaseCost.Metal * Math.Pow(2, buildingLevel),
                        BoronBuildingStorageBaseCost.Crystal * Math.Pow(2, buildingLevel),
                        BoronBuildingStorageBaseCost.Boron * Math.Pow(2, buildingLevel));
                case Buildings.ArastirmaLab:
                    return new ResourcesDTO(
                        ArastirmaLabBaseCost.Metal * Math.Pow(2, buildingLevel),
                        ArastirmaLabBaseCost.Crystal * Math.Pow(2, buildingLevel),
                        ArastirmaLabBaseCost.Boron * Math.Pow(2, buildingLevel));
                case Buildings.RobotFabrikası:
                    return new ResourcesDTO(
                        RobotFabrikasiBaseCost.Metal * Math.Pow(2, buildingLevel),
                        RobotFabrikasiBaseCost.Crystal * Math.Pow(2, buildingLevel),
                        RobotFabrikasiBaseCost.Boron * Math.Pow(2, buildingLevel));
                case Buildings.Tersane:
                    return new ResourcesDTO(
                        TersaneBaseCost.Metal * Math.Pow(2, buildingLevel),
                        TersaneBaseCost.Crystal * Math.Pow(2, buildingLevel),
                        TersaneBaseCost.Boron * Math.Pow(2, buildingLevel));
                default:
                    return ResourcesDTO.ResourceZero;
            }
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
            return ((buildingCost.Metal + buildingCost.Crystal) / ((double)2500 * (1 + robotFactoryLevel) * UniverseSpeed)) * 60;
        }

        /// <summary>
        /// Binaların saatlik üretimini hesaplıyoruz.
        /// </summary>
        /// <param name="building">Hangi bina için hesaplanacak. (Metal/Kristal/Boron)</param>
        /// <param name="buildingLevel">Binanı seviyesi.</param>
        /// <returns></returns>
        public static double GetBuildingProdPerHour(Buildings building, int buildingLevel)
        {
            switch (building)
            {
                case Buildings.MetalMadeni:
                    return MetalBuildingBasePerHour * UniverseSpeed * buildingLevel * Math.Pow(1.1f, buildingLevel) + MetalBuildingBasePerHour * UniverseSpeed;
                case Buildings.KristalMadeni:
                    return CrystalBuildingBasePerHour * UniverseSpeed * buildingLevel * Math.Pow(1.1f, buildingLevel) + CrystalBuildingBasePerHour * UniverseSpeed;
                case Buildings.BoronMadeni:
                    return BoronBuildingBasePerHour * UniverseSpeed * buildingLevel * Math.Pow(1.1f, buildingLevel) + BoronBuildingBasePerHour * UniverseSpeed;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Binanın depo kapasitesi.
        /// </summary>
        /// <param name="buildingLevel">Binanın seviyesi.</param>
        /// <returns></returns>
        public static double GetBuildingStorage(Buildings building, int buildingLevel)
        {
            switch (building)
            {
                case Buildings.MetalDeposu:
                    return MetalBuildingStorageCapacity + 50000 * (Math.Pow(1.6f, buildingLevel) - 1);
                case Buildings.KristalDeposu:
                    return CrystalBuildingStorageCapacity + 50000 * (Math.Pow(1.6f, buildingLevel) - 1);
                case Buildings.BoronDeposu:
                    return BoronBuildingStorageCapacity + 50000 * (Math.Pow(1.6f, buildingLevel) - 1);
                default:
                    return 0;
            }
        }
    }
}
