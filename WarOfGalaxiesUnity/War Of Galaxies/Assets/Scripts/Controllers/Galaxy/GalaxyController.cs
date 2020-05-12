using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyController : MonoBehaviour
{
    public static GalaxyController GC { get; set; }

    [Header("Gezegenlerin bilgisini tutuyoruz.")]
    public List<SolarPanetDataDTO> SolarPanetData;

    /// <summary>
    /// Gösterilen güneş sistemi.
    /// </summary>
    public GalaxyInfoResponseDTO SolarSystem { get; set; }

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
        LoadSolarSystem(1, 1);
    }

    void Update()
    {
        #region Galaxiyi çeviriyoruz.

        RenderSettings.skybox.SetFloat("_Rotation", Time.time * GalaxyRotationSpeed);

        #endregion
    }

    public void LoadSolarSystem(int galaxyIndex, int solarIndex)
    {
        // Eğer zaten bu kordinatta isek geri dön.
        if (SolarSystem != null && SolarSystem.GalaxyIndex == galaxyIndex && SolarSystem.SolarIndex == solarIndex)
            return;

        // Loading ekranını açıyoruz.
        LoadingController.LC.ShowLoading();

        // Var ise önceki gezegenleri temizliyoruz.
        if (currentSun != null)
        {
            currentSun.Planets.ForEach(e => Destroy(e.gameObject));
            Destroy(currentSun.gameObject);
        }

        // Kordinattaki verileri alıyoruz.
        StartCoroutine(ApiService.API.Post("GetCordinateDetails", new GalaxyInfoRequestDTO { GalaxyIndex = galaxyIndex, SolarIndex = solarIndex }, (ApiResult result) =>
        {
            // Loading ekranını açıyoruz.
            LoadingController.LC.HideLoading();

            // Galaksi bilgisini alıyoruz.
            SolarSystem = result.GetData<GalaxyInfoResponseDTO>();

            // Güneş sisteme koyuyoruz.
            GameObject sun = Instantiate(SunObject, Vector3.zero, Quaternion.identity, transform);

            // Güneş bilgisi.
            currentSun = sun.GetComponent<SunController>();

            // Gezegenleri yüklüyoruz.
            for (int ii = 0; ii < SolarSystem.SolarPlanets.Count; ii++)
            {
                // Gezegen bilgisi.
                SolarPlanetDTO solarPlanet = SolarSystem.SolarPlanets[ii];

                // Stringi kordinata çeviriyoruz.
                CordinateDTO cordinate = new CordinateDTO(SolarSystem.GalaxyIndex, SolarSystem.SolarIndex, solarPlanet.OrderIndex);

                // Her gezegen arasında 100 birim fark olacak.
                float offsetX = PlanetsDistancePer * cordinate.OrderIndex;

                // Gezegenin konumu.
                Vector3 planetPosition = new Vector3(offsetX, 0, 0);

                // Gezegen bilgisini buluyoruz.
                SolarPanetDataDTO planetData = SolarPanetData.Find(x => x.PlanetType == (PlanetTypes)solarPlanet.UserPlanet.PlanetType);

                // Gezegeni oluşturuyoruz.
                GameObject planet = Instantiate(planetData.SolarPlanet, planetPosition, Quaternion.identity, transform);

                // İzini buluyoruz.
                TrailRenderer trail = planet.GetComponent<TrailRenderer>();

                // İzin rengini değiştiriyoruz.
                trail.endColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                // Gezegeni yönetmek için bilgileri aktarmamız lazım. Componentini alıyoruz.
                PlanetController planetController = planet.GetComponent<PlanetController>();

                // Gezegen bilgisini yüklüyoruz.
                planetController.LoadPlanetInfo(currentSun, solarPlanet.UserPlanet, cordinate);

                // Oluşan gezegeni ekliyoruz.
                currentSun.AddPlanet(planetController);
            }
        }));
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
