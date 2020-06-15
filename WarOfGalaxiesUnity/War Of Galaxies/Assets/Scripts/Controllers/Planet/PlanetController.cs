using Assets.Scripts.ApiModels;
using Assets.Scripts.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetController : MonoBehaviour
{
    [Header("Yörüngesindeki güneş.")]
    public SunController Sun;

    [Header("Güneşin etrafındaki dönüş hızı.")]
    public float RotateSunsAroundSpeed = 5;

    [Header("Gezegenin kullanıcı bilgisi.")]
    public GameObject UserPlanetInfo;

    [Header("Gezegende enkaz olduğunda burası dolu olacak.")]
    public Transform DebrisEffect;

    /// <summary>
    /// Tutulan gezegene ait bilgiler. Var ise.
    /// </summary>
    public SolarPlanetDTO SolarPlanetInfo { get; set; }

    /// <summary>
    /// Kordinat bilgisi.
    /// </summary>
    public CordinateDTO CordinateInfo { get; set; }

    public void LoadPlanetInfo(SunController sun, SolarPlanetDTO solarPlanet, CordinateDTO cordinate)
    {
        // Güneşi atıyoruz.
        Sun = sun;

        // Gezegen bilgisini atıyoruz.
        SolarPlanetInfo = solarPlanet;

        // Kordinat bilgisini atıyoruz.
        CordinateInfo = cordinate;

        // Güneşin etrafında rastgele bir konuma atıyoruz.
        transform.RotateAround(Sun.transform.position, transform.up, Random.Range(1, 360));

        // Eğer gezegen bilgisi var ise yüklüyoruz.
        if (solarPlanet != null && solarPlanet.UserPlanet != null)
        {
            UserPlanetInfo = Instantiate(GalaxyController.GC.UserPlanetInfo, transform);
            UserPlanetInfo.transform.Find("TXT_Username").GetComponent<TMP_Text>().text = solarPlanet.UserPlanet.PlanetName;
            UserPlanetInfo.transform.Find("TXT_OrderIndex").GetComponent<TMP_Text>().text = cordinate.OrderIndex.ToString();
            UserPlanetInfo.transform.Find("TXT_UserOrder").GetComponent<TMP_Text>().text = "-";

            // Enkazın toplamı.
            double sumOfDebris = solarPlanet.GarbageMetal + solarPlanet.GarbageCrystal + solarPlanet.GarbageBoron;

            // Eğer gezegende enkaz var ise enkaz oluşturuyoruz.
            if (sumOfDebris > 0)
            {
                // Çokta fazla olmayacak şekilde asteroid gösteceğiz. 100k ve üzerinde her zaman 100 tane gösterilecek.
                if (sumOfDebris > 100000)
                    sumOfDebris = 100000;

                // Particle oluşturuyoruz.
                DebrisEffect = Instantiate(GalaxyController.GC.Debris, transform).transform;

                // Particle alıyoruz.
                ParticleSystem ps = DebrisEffect.GetComponent<ParticleSystem>();

                // Effecti veriyoruz.
                ParticleSystem.MainModule effect = ps.main;

                // Ne kadar yoğun gösterileceğini hesaplıyoruz.
                effect.maxParticles = (int)(GlobalGalaxyController.GGC.DebrisMaxQuantity * (sumOfDebris / 100000));
            }
        }
    }

    private void Update()
    {
        // Kendi etrafında döndürüyoruz.
        if (!PlanetZoomController.PZC.IsPlanetSelected(this))
            transform.RotateAround(transform.position, transform.up, CordinateInfo.OrderIndex * Time.deltaTime);

        // Güneşin etrafında çeviriyoruz.
        if (PlanetZoomController.PZC.ZoomState == PlanetZoomController.ZoomStates.ZoomedOut)
        {
            transform.RotateAround(Sun.transform.position, transform.up, RotateSunsAroundSpeed * Time.deltaTime);
            if (UserPlanetInfo != null)
                UserPlanetInfo.transform.LookAt(ZoomPanController.ZPC.MainCamera.transform);
        }

        // Enkazı var ise döndürüyoruz.
        if (DebrisEffect != null)
            DebrisEffect.RotateAround(DebrisEffect.position, -DebrisEffect.forward, Time.deltaTime * 18);

    }

    public void OnMouseDown()
    {
        // Eğer panel açık ise geri dön.
        if (GlobalPanelController.GPC.IsAnyPanelOpen)
            return;

        // Eğer bir panele tıklanıyor ise geri dön.
        if (EventSystem.current.currentSelectedGameObject != null)
            return;

        // Eğer giriş yapılmadıysa geri dön.
        if (!LoginController.LC.IsLoggedIn)
            return;

        // Üzerine tıklandığında fokuslanıyoruz.
        //if (PlanetZoomController.PZC.ZoomState != PlanetZoomController.ZoomStates.Zoomed)
        //    PlanetZoomController.PZC.BeginZoom(this);

        // Önceden açık var ise kapatıyoruz.
        GlobalPanelController.GPC.ClosePanel(GlobalPanelController.PanelTypes.PlanetActionFooterPanel);

        //Paneli açıyoruz.
        GameObject actionPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.PlanetActionFooterPanel);

        // Filo hareketini belirliyoruz.
        actionPanel.GetComponent<PlanetActionFooterPanel>().ShowCordinate(this.SolarPlanetInfo, this.CordinateInfo);
    }
}
