using System;
using System.Collections.Generic;
using WarOfGalaxiesApi.DTO.Enums;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class DefenseDTO
    {
        /// <summary>
        /// Savunmanın idsi.
        /// </summary>
        public Defenses DefenseID { get; set; }

        /// <summary>
        /// Savunmanın taban maliyeti.
        /// </summary>
        public ResourcesDTO Cost { get; set; }

        /// <summary>
        /// Bir vuruşta kaç tane gemiyi yok edebilir.
        /// </summary>
        public List<Tuple<Ships, int>> KillInOneShot { get; set; }

        public DefenseDTO()
        {
            KillInOneShot = new List<Tuple<Ships, int>>();
        }
    }
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
