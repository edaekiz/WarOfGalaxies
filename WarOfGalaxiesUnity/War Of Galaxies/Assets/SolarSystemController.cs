using Assets.Models;
using UnityEngine;

public class SolarSystemController : MonoBehaviour
{
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
