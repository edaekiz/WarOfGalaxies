using Assets.Scripts.Enums;
using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class FleetDTO
    {
        public int FleetId;

        public int ReturnFleetId;
        public FleetTypes FleetActionTypeId;
        public string SenderCordinate;
        public int SenderUserId;
        public int SenderUserPlanetId;
        public int SenderPlanetTypeId;
        public int DestinationUserId;
        public int DestinationUserPlanetId;
        public string DestinationCordinate;
        public int DestinationPlanetTypeId;
        public double BeginPassedTime;
        public double EndLeftTime;
        public string SenderPlanetName;
        public string DestinationPlanetName;
        public double CarriedMetal;
        public double CarriedCrystal;
        public double CarriedBoron;
        public string FleetData;
        public DateTime FleetLoadDate;

        /// <summary>
        /// Eğer dönen bir filo ise değer true dönecek.
        /// </summary>
        public bool IsReturnFleet => ReturnFleetId == 0;

        public FleetDTO()
        {
            FleetLoadDate = DateTime.Now;
        }
    }

    [Serializable]
    public class GetLastFleetsDTO
    {
        public int LastFleetId;
    }

    [Serializable]
    public class SendFleetFromPlanetDTO
    {
        public int SenderUserPlanetId;
        public string Ships;
        public int DestinationGalaxyIndex;
        public int DestinationSolarIndex;
        public int DestinationOrderIndex;
        public int FleetType;
        public double CarriedMetal;
        public double CarriedCrystal;
        public double CarriedBoron;

        /// <summary>
        /// 0 ile 1 arasında.
        /// </summary>
        public float FleetSpeed;
    }
}
