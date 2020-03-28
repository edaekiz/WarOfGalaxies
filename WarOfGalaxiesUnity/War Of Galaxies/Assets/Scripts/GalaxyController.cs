using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyController : MonoBehaviour
{
    public static GalaxyController GC { get; set; }

    [Header("Gezegenlerin bilgisini tutuyoruz.")]
    public List<SolarPanetDataDTO> SolarPanetData;

    [Header("Sunucudaki gezegenlerin listesi")]
    public SolarSystemDTO SolarSystem;

    [Header("Gezegen modeli")]
    public GameObject PlanetObject;

    [Header("Güneş modeli")]
    public GameObject SunObject;

    private void Awake()
    {
        if (GC == null)
            GC = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadSolarSystem();
    }

    public void LoadSolarSystem()
    {
        // Güneş sisteme koyuyoruz.
        GameObject sun = Instantiate(SunObject, Vector3.zero, Quaternion.identity, transform);

        // Gezegenleri yüklüyoruz.
        for (int ii = 0; ii < SolarSystem.Planets.Count; ii++)
        {
            SolarPlanetDTO solarPlanet = SolarSystem.Planets[ii];
            int offsetX = 100 * solarPlanet.PlanetIndexInSolarSystem;
            Vector3 planetPosition = new Vector3(offsetX, 0, 0);
            SolarPanetDataDTO planetData = SolarPanetData.Find(x => x.SolarPlanetType == solarPlanet.SolarPlanetType);
            GameObject planet = Instantiate(PlanetObject, planetPosition, Quaternion.identity, transform);
            planet.GetComponent<MeshRenderer>().material = planetData.SolarPlanetMaterial;
            PlanetController planetController = planet.GetComponent<PlanetController>();
            planetController.LoadPlanetInfo(sun, solarPlanet);
        }
    }

    public void DisableTouchSystem()
    {
        GetComponent<Lean.Touch.LeanDragTranslate>().enabled = false;
        GetComponent<Lean.Touch.LeanTwistRotate>().enabled = false;
        GetComponent<Lean.Touch.LeanPinchScale>().enabled = false;
        Camera.main.GetComponent<Lean.Touch.LeanDragCamera>().enabled = false;
    }

    public void EnableTouchSystem()
    {
        GetComponent<Lean.Touch.LeanDragTranslate>().enabled = true;
        GetComponent<Lean.Touch.LeanTwistRotate>().enabled = true;
        GetComponent<Lean.Touch.LeanPinchScale>().enabled = true;
        Camera.main.GetComponent<Lean.Touch.LeanDragCamera>().enabled = true;
    }

}
