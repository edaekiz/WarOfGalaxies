using System;
using System.Collections.Generic;
using WarOfGalaxiesApi.DTO.Enums;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class ShipDTO
    {
        /// <summary>
        /// Geminini idsi.
        /// </summary>
        public Ships ShipID { get; set; }

        /// <summary>
        /// Geminin taban maliyeti.
        /// </summary>
        public ResourcesDTO Cost { get; set; }

        /// <summary>
        /// Bir vuruşta kaç tane gemiyi yok edebilir.
        /// </summary>
        public List<Tuple<Ships, int>> KillInOneShot { get; set; }

        /// <summary>
        /// Geminin kargo kapasitesi.
        /// </summary>
        public int CargoCapacity { get; set; }

        /// <summary>
        /// Geminin yakıtı.
        /// </summary>
        public int ShipFuel { get; set; }

        /// <summary>
        /// Gemi hızı. 1000 demek 1 sonraki sıradaki gezegene 10dk da gidiyor demek.
        /// </summary>
        public int ShipSpeed { get; set; }

        public ShipDTO()
        {
            KillInOneShot = new List<Tuple<Ships, int>>();
        }
    }
}
