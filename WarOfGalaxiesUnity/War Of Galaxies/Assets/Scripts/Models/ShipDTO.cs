using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

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
        /// Gemi hızı. 1000 demek 1 sonraki sıradaki gezegene 10dk da gidiyor demek.
        /// </summary>
        public int ShipSpeed;

        /// <summary>
        /// Temel yakıt değeri.
        /// </summary>
        public int BaseFuelt;

        public ShipDTO()
        {
            KillInOneShot = new List<Tuple<Ships, int>>();
        }

    }
}
