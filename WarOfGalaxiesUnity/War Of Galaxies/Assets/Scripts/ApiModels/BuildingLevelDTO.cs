using Assets.Scripts.Enums;
using System;

namespace Assets.Scripts.ApiModels
{
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
}
