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

    [Header("Güneş modeli")]
    public GameObject SunObject;

    [Header("Fpsı bastığımız field.")]
    public Text FpsText;

    private float deltaTime;

    // Güneş bilgisi.
    private SunController currentSun;

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

        // Güneş bilgisi.
        currentSun = sun.GetComponent<SunController>();

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
            planetController.LoadPlanetInfo(currentSun, solarPlanet);

            // Oluşan gezegeni ekliyoruz.
            currentSun.AddPlanet(planetController);
        }

        // Lineları yüklüyoruz.
        currentSun.LoadLines(SolarSystem.Planets.Select(x => x.PlanetIndexInSolarSystem).ToArray());

    }

    /// <summary>
    /// Galaksi üzerinde hareket etme, döndürme, zoom yapma gibi özellikleri kapatıyoruz.
    /// </summary>
    public void DisableTouchSystem()
    {
        // Kamera kontrolünü kapatıyoruz.
        ZoomPanController.ZPC.ZoomPanEnabled = false;

        // Lineları devre dışı bırakıyoruz.
        currentSun.DisableLines();
    }

    /// <summary>
    /// Galaksi üzerinde hareket etme, döndürme, zoom yapma gibi özelliklerini açıyoruz.
    /// </summary>
    public void EnableTouchSystem()
    {
        // Kamera kontrolü açıyoruz.
        ZoomPanController.ZPC.ZoomPanEnabled = true;

        // Lineları tekrar açıyoruz.
        currentSun.EnableLines();
    }

}
