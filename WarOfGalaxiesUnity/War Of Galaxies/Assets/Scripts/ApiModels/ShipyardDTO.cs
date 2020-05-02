using Assets.Scripts.Enums;
using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class ShipyardAddQueueRequestDTO
    {
        public int UserPlanetID;
        public Ships ShipID;
        public int Quantity;
    }

    [Serializable]
    public class ShipyardAddQueueResponseDTO
    {
        public Ships ShipID;
        public int Quantity;
        public ResourcesDTO PlanetResources;
        public int UserPlanetID;
    }

    [Serializable]
    public class UserPlanetShipProgDTO
    {
        public int UserPlanetId;
        public Ships ShipId;
        public int ShipCount;
        public int OrderIndex;
    }

    [Serializable]
    public class UserPlanetShipDTO
    {
        public int UserPlanetId;
        public Ships ShipId;
        public int ShipCount;
    }
}
