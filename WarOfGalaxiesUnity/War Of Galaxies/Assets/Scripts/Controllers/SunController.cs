using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    public float RotateSpeed;
    public int OrbitalDetail;
    public GameObject OrbitLine;
    public Transform lineContent;
    public GameObject lineMover;

    public List<PlanetController> Planets = new List<PlanetController>();

    // Update is called once per frame
    void Update()
    {
        // Kendi etrafında döndürüyoruz.
        transform.RotateAround(transform.position, transform.up, RotateSpeed * Time.deltaTime);
    }

    public void AddPlanet(PlanetController planet)
    {
        Planets.Add(planet);
    }

    public void DisableNotSelectedPlanets(PlanetController selectedPlanet)
    {
        // Gezgenleri kapatıyoruz.
        Planets.ForEach(e =>
        {
            // Seçili olan dışındaki gezegenleri kapatıyoruz.
            if (!ReferenceEquals(e, selectedPlanet))
                e.gameObject.SetActive(false);
        });

        // Güneşi de kapatıyoruz.
        gameObject.SetActive(false);
    }

    public void EnableAllPlanets()
    {
        // Gezegenleri açıyoruz.
        Planets.ForEach(e => e.gameObject.SetActive(true));

        // Güneşi de açıyoruz.
        gameObject.SetActive(true);
    }

}
