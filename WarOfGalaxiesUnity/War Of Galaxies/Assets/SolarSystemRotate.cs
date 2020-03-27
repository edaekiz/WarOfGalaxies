using System;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemRotate : MonoBehaviour
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

    public SolarSystem System;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int ii = 0; ii < System.Planets.Count; ii++)
        {
            SolarPlanet planet = System.Planets[ii];
            planet.Planet.transform.RotateAround(transform.position, transform.up,planet.PlanetRotationSpeed);
        }
    }
}
