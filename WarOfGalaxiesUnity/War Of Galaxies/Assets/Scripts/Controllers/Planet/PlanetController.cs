using Assets.Scripts.ApiModels;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetController : MonoBehaviour
{
    [Header("Yörüngesindeki güneş.")]
    public SunController Sun;

    [Header("Gösterdiği gezegen bilgisi.")]
    public UserPlanetDTO SolarPlanetInfo;

    [Header("Gezegenin sistemdeki kordinatları.")]
    public CordinateDTO CordinateInfo;

    [Header("Güneşin etrafındaki dönüş hızı.")]
    public float RotateSunsAroundSpeed = 5;

    public void LoadPlanetInfo(SunController sun, UserPlanetDTO solarPlanet, CordinateDTO cordinate)
    {
        // Güneşi atıyoruz.
        Sun = sun;

        // Gezegen bilgisini atıyoruz.
        SolarPlanetInfo = solarPlanet;

        // Kordinat bilgisini atıyoruz.
        CordinateInfo = cordinate;

        // Güneşin etrafında rastgele bir konuma atıyoruz.
        transform.RotateAround(Sun.transform.position, transform.up, Random.Range(1, 360));
    }

    private void Update()
    {
        // Eğer gezegen bilgisi tanımlı değil ise geri dön.
        if (SolarPlanetInfo == null)
            return;

        // Kendi etrafında döndürüyoruz.
        if (!PlanetZoomController.PZC.IsPlanetSelected(this))
            transform.RotateAround(transform.position, transform.up, CordinateInfo.OrderIndex * Time.deltaTime);

        // Güneşin etrafında çeviriyoruz.
        if (PlanetZoomController.PZC.ZoomState == PlanetZoomController.ZoomStates.ZoomedOut)
            transform.RotateAround(Sun.transform.position, transform.up, RotateSunsAroundSpeed * Time.deltaTime);
    }

    public void OnMouseDown()
    {
        // Eğer panel açık ise geri dön.
        if (GlobalPanelController.GPC.IsAnyPanelOpen)
            return;

        // Eğer bir panele tıklanıyor ise geri dön.
        if (EventSystem.current.currentSelectedGameObject != null)
            return;

        // Üzerine tıklandığında fokuslanıyoruz.
        if (PlanetZoomController.PZC.ZoomState != PlanetZoomController.ZoomStates.Zoomed)
            PlanetZoomController.PZC.BeginZoom(this);
    }
}
