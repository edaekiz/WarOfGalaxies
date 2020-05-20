using System;
using System.Collections.Generic;
using WarOfGalaxiesApi.DTO.Enums;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class DefenseAddQueueRequestDTO
    {
        public int UserPlanetID { get; set; }
        public Defenses DefenseID { get; set; }
        public int Quantity { get; set; }
    }

    public class DefenseAddQueueResponseDTO
    {
        public int DefenseID { get; set; }
        public int Quantity { get; set; }
        public ResourcesDTO PlanetResources { get; set; }
        public int UserPlanetID { get; set; }
    }

    public class UserPlanetDefenseProgDTO
    {
        public int UserPlanetId { get; set; }
        public int DefenseId { get; set; }
        public int DefenseCount { get; set; }
        public double OffsetTime { get; set; }
        public DateTime? LastVerifyDate { get; set; }
    }

    public class UserPlanetDefenseDTO
    {
        public int UserPlanetId { get; set; }
        public int DefenseId { get; set; }
        public int DefenseCount { get; set; }
    }
}
