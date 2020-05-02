using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class ShipDTO
    {
        /// <summary>
        /// Geminini idsi.
        /// </summary>
        public Ships ShipID;

        /// <summary>
        /// Geminin taban maliyeti.
        /// </summary>
        public ResourcesDTO Cost;

        /// <summary>
        /// Bir vuruşta kaç tane gemiyi yok edebilir.
        /// </summary>
        public List<Tuple<Ships, int>> KillInOneShot;

        /// <summary>
        /// Geminin kargo kapasitesi.
        /// </summary>
        public int CargoCapacity;

        /// <summary>
        /// Geminin yakıtı.
        /// </summary>
        public int ShipFuel;

        public ShipDTO()
        {
            KillInOneShot = new List<Tuple<Ships, int>>();
        }

    }
}
