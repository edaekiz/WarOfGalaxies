using Assets.Scripts.ApiModels.Base;
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
    public class UserPlanetBuildingUpgDTO : ProgressModel
    {
        public ResourcesDTO PlanetResources;
        public int UserPlanetId;
        public Buildings BuildingId;
        public int BuildingLevel;
        public double LeftTime;
        public double PassedTime;
    }

    [Serializable]
    public class UserPlanetUpgradeBuildingDTO
    {
        public int UserPlanetID;
        public int BuildingID;
    }
}
