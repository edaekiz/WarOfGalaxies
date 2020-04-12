using Assets.Scripts.Enums;
using System;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class SolarPlanetDTO
    {
        public int UserPlanetID;
        public string UserName;
        public string PlanetName;
        public string PlanetCordinate;
        public PlanetTypes PlanetType;
    }

    [Serializable]
    public class SolarPanetDataDTO
    {
        public PlanetTypes PlanetType;
        public GameObject SolarPlanet;
    }
}
