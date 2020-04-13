using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class UserPlanetBuildingDTO
    {
        public int UserPlanetID;
        public Buildings BuildingID;
        public int BuildingLevel;
    }

    [Serializable]
    public class BuildingLevelDTO
    {
        public Buildings BuildingID;
        public int BuildingLevel;
        public long UpgradeTime;
        public int BuildingValue;
        public int RequiredMetal;
        public int RequiredCrystal;
        public int RequiredBoron;
        public int RequiredEnergy;
    }
    public static class Data
    {

        public static List<UserPlanetBuildingDTO> UserPlanetBuidings = new List<UserPlanetBuildingDTO>
        {
            new UserPlanetBuildingDTO
            {
                BuildingID = Buildings.MetalMadeni,
                BuildingLevel = 1,
                UserPlanetID = 1
            },
            new UserPlanetBuildingDTO
            {
                BuildingID = Buildings.MetalDeposu,
                BuildingLevel = 1,
                UserPlanetID = 1
            }
        };

        public static List<BuildingLevelDTO> BuildingLevels = new List<BuildingLevelDTO>
        {
            new BuildingLevelDTO
            {
                BuildingID = Buildings.MetalMadeni,
                BuildingLevel = 1,
                BuildingValue = 300,
                RequiredBoron = 0,
                RequiredMetal = 250,
                RequiredCrystal = 100,
                RequiredEnergy = 25,
                UpgradeTime = 5
            },
            new BuildingLevelDTO
            {
                BuildingID = Buildings.MetalMadeni,
                BuildingLevel = 2,
                BuildingValue = 450,
                RequiredBoron = 0,
                RequiredMetal = 300,
                RequiredCrystal = 125,
                RequiredEnergy = 25,
                UpgradeTime = 7
            },
            new BuildingLevelDTO
            {
                BuildingID = Buildings.MetalDeposu,
                BuildingLevel = 1,
                BuildingValue = 5000,
                RequiredBoron = 0,
                RequiredMetal = 250,
                RequiredCrystal = 100,
                RequiredEnergy = 0,
                UpgradeTime = 5
            },
            new BuildingLevelDTO
            {
                BuildingID = Buildings.MetalDeposu,
                BuildingLevel = 2,
                BuildingValue = 10000,
                RequiredBoron = 0,
                RequiredMetal = 300,
                RequiredCrystal = 125,
                RequiredEnergy = 0,
                UpgradeTime = 7
            },
        };
    }
}
