using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class SolarSystemDTO
    {
        public int GalaxyIndex;
        public int SolarSystemIndex;
        public List<SolarPlanetDTO> Planets;
        public SolarSystemDTO()
        {
            Planets = new List<SolarPlanetDTO>();
        }
    }

    [Serializable]
    public class SolarPlanetDTO
    {
        public int PlanetIndexInSolarSystem;
        public SolarPlanetTypes SolarPlanetType;
        public float AroundRotateSpeed;
        public float SunAroundRotateSpeed;
    }

    [Serializable]
    public enum SolarPlanetTypes
    {
        VENUS = 1,
        MARS = 2,
        MERKURY = 3,
        SATURN = 4,
        JUPITER = 5,
        NEPTUNE = 6,
        URANUS = 7
    }

    [Serializable]
    public class SolarPanetDataDTO
    {
        public SolarPlanetTypes SolarPlanetType;
        public GameObject SolarPlanet;
    }
}
