using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GalaxyController : MonoBehaviour
{
    public static GalaxyController GC { get; set; }

    [Header("Gezegenlerin bilgisini tutuyoruz.")]
    public List<SolarPanetDataDTO> SolarPanetData;

    [Header("Sunucudaki gezegenlerin listesi")]
    public List<SolarPlanetDTO> SolarSystem;

    [Header("Her bir gezegen arasındaki mesafe.")]
    public float PlanetsDistancePer;

    [Header("Güneş modeli")]
    public GameObject SunObject;

    [Header("Galaksinin dönüş hızı.")]
    public float GalaxyRotationSpeed;

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
        #region Galaxiyi çeviriyoruz.

        RenderSettings.skybox.SetFloat("_Rotation", Time.time * GalaxyRotationSpeed);

        #endregion
    }

    public void LoadSolarSystem()
    {
        // Güneş sisteme koyuyoruz.
        GameObject sun = Instantiate(SunObject, Vector3.zero, Quaternion.identity, transform);

        // Güneş bilgisi.
        currentSun = sun.GetComponent<SunController>();

        // Gezegenleri yüklüyoruz.
        for (int ii = 0; ii < SolarSystem.Count; ii++)
        {
            // Gezegen bilgisi.
            SolarPlanetDTO solarPlanet = SolarSystem[ii];

            // Stringi kordinata çeviriyoruz.
            CordinateDTO cordinate = solarPlanet.PlanetCordinate.ToCordinate();

            // Her gezegen arasında 100 birim fark olacak.
            float offsetX = PlanetsDistancePer * cordinate.SolarSystemOrderIndex;

            // Gezegenin konumu.
            Vector3 planetPosition = new Vector3(offsetX, 0, 0);

            // Gezegen bilgisini buluyoruz.
            SolarPanetDataDTO planetData = SolarPanetData.Find(x => x.PlanetType == solarPlanet.PlanetType);

            // Gezegeni oluşturuyoruz.
            GameObject planet = Instantiate(planetData.SolarPlanet, planetPosition, Quaternion.identity, transform);

            // İzini buluyoruz.
            TrailRenderer trail = planet.GetComponent<TrailRenderer>();

            // İzin rengini değiştiriyoruz.
            trail.endColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            // Gezegeni yönetmek için bilgileri aktarmamız lazım. Componentini alıyoruz.
            PlanetController planetController = planet.GetComponent<PlanetController>();

            // Gezegen bilgisini yüklüyoruz.
            planetController.LoadPlanetInfo(currentSun, solarPlanet, cordinate);

            // Oluşan gezegeni ekliyoruz.
            currentSun.AddPlanet(planetController);
        }
    }

    /// <summary>
    /// Galaksi üzerinde hareket etme, döndürme, zoom yapma gibi özellikleri kapatıyoruz.
    /// </summary>
    public void DisableTouchSystem()
    {
        // Kamera kontrolünü kapatıyoruz.
        ZoomPanController.ZPC.ZoomPanEnabled = false;
    }

    /// <summary>
    /// Galaksi üzerinde hareket etme, döndürme, zoom yapma gibi özelliklerini açıyoruz.
    /// </summary>
    public void EnableTouchSystem()
    {
        // Kamera kontrolü açıyoruz.
        ZoomPanController.ZPC.ZoomPanEnabled = true;
    }

}
