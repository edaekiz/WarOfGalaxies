using System;
using WarOfGalaxiesApi.DTO.Enums;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class ShipyardAddQueueRequestDTO
    {
        public int UserPlanetID { get; set; }
        public Ships ShipID { get; set; }
        public int Quantity { get; set; }
    }

    public class ShipyardAddQueueResponseDTO
    {
        public int ShipID { get; set; }
        public int Quantity { get; set; }
        public ResourcesDTO PlanetResources { get; set; }
        public int UserPlanetID { get; set; }
    }

    public class UserPlanetShipProgDTO
    {
        public int UserPlanetId { get; set; }
        public int ShipId { get; set; }
        public int ShipCount { get; set; }
        public int OrderIndex { get; set; }
    }

    public class UserPlanetShipDTO
    {
        public int UserPlanetId { get; set; }
        public int ShipId { get; set; }
        public int ShipCount { get; set; }
    }

}
