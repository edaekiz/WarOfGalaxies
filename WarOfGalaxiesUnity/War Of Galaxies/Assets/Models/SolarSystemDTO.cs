using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Models
{
    [Serializable]
    public class SolarSystem
    {
        public List<SolarPlanet> Planets;
        public SolarSystem()
        {
            Planets = new List<SolarPlanet>();
        }
    }
    [Serializable]
    public class SolarPlanet
    {
        public GameObject Planet;
        public float PlanetRotationSpeed;
        public float DistanceFromSun;
    }
}
