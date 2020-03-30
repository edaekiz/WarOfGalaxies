using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyController : MonoBehaviour
{
    public static GalaxyController GC { get; set; }

    [Header("Gezegenlerin bilgisini tutuyoruz.")]
    public List<SolarPanetDataDTO> SolarPanetData;

    [Header("Sunucudaki gezegenlerin listesi")]
    public SolarSystemDTO SolarSystem;

    [Header("Her bir gezegen arasındaki mesafe.")]
    public float PlanetsDistancePer;

    [Header("Gezegen modeli")]
    public GameObject PlanetObject;

    [Header("Güneş modeli")]
    public GameObject SunObject;

    public Text FpsText;

    private void Awake()
    {
        if (GC == null)
            GC = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Application.targetFrameRate = 300;
        LoadSolarSystem();
    }

    public float deltaTime;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        FpsText.text = Mathf.Ceil(fps).ToString();
    }

    public void LoadSolarSystem()
    {
        // Güneş sisteme koyuyoruz.
        GameObject sun = Instantiate(SunObject, Vector3.zero, Quaternion.identity, transform);

        // Gezegenleri yüklüyoruz.
        for (int ii = 0; ii < SolarSystem.Planets.Count; ii++)
        {
            // Gezegen bilgisi.
            SolarPlanetDTO solarPlanet = SolarSystem.Planets[ii];

            // Her gezegen arasında 100 birim fark olacak.
            float offsetX = PlanetsDistancePer * solarPlanet.PlanetIndexInSolarSystem;

            // Gezegenin konumu.
            Vector3 planetPosition = new Vector3(offsetX, 0, 0);

            // Gezegen bilgisini buluyoruz.
            SolarPanetDataDTO planetData = SolarPanetData.Find(x => x.SolarPlanetType == solarPlanet.SolarPlanetType);

            // Gezegeni oluşturuyoruz.
            GameObject planet = Instantiate(planetData.SolarPlanet, planetPosition, Quaternion.identity, transform);

            // Gezegeni yönetmek için bilgileri aktarmamız lazım. Componentini alıyoruz.
            PlanetController planetController = planet.GetComponent<PlanetController>();

            // Gezegen bilgisini yüklüyoruz.
            planetController.LoadPlanetInfo(sun, solarPlanet);
        }

        sun.GetComponent<SunController>().LoadLines(SolarSystem.Planets.Select(x => x.PlanetIndexInSolarSystem).ToArray());

    }

    /// <summary>
    /// Galaksi üzerinde hareket etme, döndürme, zoom yapma gibi özellikleri kapatıyoruz.
    /// </summary>
    public void DisableTouchSystem()
    {
        GetComponent<Lean.Touch.LeanDragTranslate>().enabled = false;
        GetComponent<Lean.Touch.LeanPinchScale>().enabled = false;
    }

    /// <summary>
    /// Galaksi üzerinde hareket etme, döndürme, zoom yapma gibi özelliklerini açıyoruz.
    /// </summary>
    public void EnableTouchSystem()
    {
        GetComponent<Lean.Touch.LeanDragTranslate>().enabled = true;
        GetComponent<Lean.Touch.LeanPinchScale>().enabled = true;
    }

}
