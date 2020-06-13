using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyController : MonoBehaviour
{
    public static GalaxyController GC { get; set; }

    [Header("Gezegenlerin bilgisini tutuyoruz.")]
    public List<SolarPanetDataDTO> SolarPanetData;

    [Header("Her bir gezegen arasındaki mesafe.")]
    public float PlanetsDistancePer;

    [Header("Güneş modeli")]
    public GameObject SunObject;

    [Header("Galakside her bir gezegen için gösterilecek olan gezegen bilgisi.")]
    public GameObject UserPlanetInfo;

    [Header("Galaksinin dönüş hızı.")]
    public float GalaxyRotationSpeed;

    [Header("Boş gezegen")]
    public GameObject EmptyPlanetSign;

    [Header("Gezegende enkaz var ise bu modeli oluşturacağız.")]
    public GameObject Debris;

    /// <summary>
    /// Gösterilen güneş sistemi.
    /// </summary>
    public GalaxyInfoResponseDTO SolarSystem { get; set; }

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
        StartCoroutine(GoCurrentPlanetCordinate());
    }

    private IEnumerator GoCurrentPlanetCordinate()
    {
        // Oyun yüklendiğinde çalışacak.
        yield return new WaitUntil(() => LoadingController.LC.IsGameLoaded);

        // Kordinatı alıyoruz ilk açılışta.
        yield return new WaitUntil(() => GlobalPlanetController.GPC.CurrentPlanetCordinate != null);

        // Gezegenin kordinatını yüklüyoruz.
        LoadSolarSystem(GlobalPlanetController.GPC.CurrentPlanetCordinate.GalaxyIndex, GlobalPlanetController.GPC.CurrentPlanetCordinate.SolarIndex);
    }

    void Update()
    {
        #region Galaxiyi çeviriyoruz.

        RenderSettings.skybox.SetFloat("_Rotation", Time.time * GalaxyRotationSpeed);

        #endregion
    }

    public void LoadSolarSystem(int galaxyIndex, int solarIndex)
    {
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
            if (result.IsSuccess != true)
                return;

            // Loading ekranını açıyoruz.
            LoadingController.LC.CloseLoading();

            // Galaksi bilgisini alıyoruz.
            SolarSystem = result.GetData<GalaxyInfoResponseDTO>();

            // Güneş sisteme koyuyoruz.
            GameObject sun = Instantiate(SunObject, Vector3.zero, Quaternion.identity, transform);

            // Güneş bilgisi.
            currentSun = sun.GetComponent<SunController>();

            // Gezegenleri yüklüyoruz.
            for (int ii = 1; ii <= 8; ii++)
            {

                // Gezegen bilgisi.
                SolarPlanetDTO solarPlanet = SolarSystem.SolarPlanets.Find(x => x.OrderIndex == ii);

                // Stringi kordinata çeviriyoruz.
                CordinateDTO cordinate = new CordinateDTO(SolarSystem.GalaxyIndex, SolarSystem.SolarIndex, ii);

                // Her gezegen arasında 100 birim fark olacak.
                float offsetX = PlanetsDistancePer * cordinate.OrderIndex;

                // Gezegenin konumu.
                Vector3 planetPosition = new Vector3(offsetX, 0, 0);

                // Sahiplenilmemiş gezegen.
                if (solarPlanet == null)
                {
                    // Gezegeni oluşturuyoruz.
                    GameObject planet = Instantiate(EmptyPlanetSign, planetPosition, Quaternion.identity, transform);

                    // İzini buluyoruz.
                    TrailRenderer trail = planet.GetComponent<TrailRenderer>();

                    // İzin rengini değiştiriyoruz.
                    trail.endColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                    // Gezegeni yönetmek için bilgileri aktarmamız lazım. Componentini alıyoruz.
                    PlanetController planetController = planet.GetComponent<PlanetController>();

                    // Gezegen bilgisini yüklüyoruz.
                    planetController.LoadPlanetInfo(currentSun, null, cordinate);

                    // Oluşan gezegeni ekliyoruz.
                    currentSun.AddPlanet(planetController);
                }
                else
                {
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
                    planetController.LoadPlanetInfo(currentSun, solarPlanet, cordinate);

                    // Oluşan gezegeni ekliyoruz.
                    currentSun.AddPlanet(planetController);
                }
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
