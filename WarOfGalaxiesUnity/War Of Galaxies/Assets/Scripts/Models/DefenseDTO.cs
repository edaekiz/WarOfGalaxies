using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class DefenseDTO
    {
        /// <summary>
        /// Savunmanın idsi.
        /// </summary>
        public Defenses DefenseID;

        /// <summary>
        /// Savunmanın taban maliyeti.
        /// </summary>
        public ResourcesDTO Cost;

        /// <summary>
        /// Bir vuruşta kaç tane gemiyi yok edebilir.
        /// </summary>
        public List<Tuple<Ships, int>> KillInOneShot;

        public DefenseDTO()
        {
            KillInOneShot = new List<Tuple<Ships, int>>();
        }
    }
}
