using Assets.Scripts.Enums;
using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class UserPlanetBuildingDTO
    {
        public int UserPlanetId;
        public Buildings BuildingId;
        public int BuildingLevel;
    }

    [Serializable]
    public class UserPlanetBuildingUpgDTO
    {
        public int UserPlanetId;
        public Buildings BuildingId;
        public int BuildingLevel;
        public double LeftTime;
    }
}
