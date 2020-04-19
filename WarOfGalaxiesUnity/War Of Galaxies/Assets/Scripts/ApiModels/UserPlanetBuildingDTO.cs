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
        public DateTime BeginDate;
        public DateTime EndDate;

        public void CalculateDates()
        {
            BeginDate = DateTime.UtcNow;
            EndDate = BeginDate.AddSeconds(LeftTime);
        }
    }

    [Serializable]
    public class UserPlanetUpgradeBuildingDTO
    {
        public int UserPlanetID;
        public Buildings BuildingID;
    }
}
