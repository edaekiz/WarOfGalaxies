using Assets.Scripts.Models;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [Header("Yörüngesindeki güneş")]
    public SunController Sun;

    [Header("Gösterdiği gezegen bilgisi.")]
    public SolarPlanetDTO SolarPlanetInfo;

    private float rotateSelfSpeed;
    private float rotateAroundSunSpeed;

    public void LoadPlanetInfo(SunController sun, SolarPlanetDTO solarPlanet)
    {
        // Güneşi atıyoruz.
        Sun = sun;

        // Gezegen bilgisini atıyoruz.
        SolarPlanetInfo = solarPlanet;

        // Güneşin etrafında rastgele bir konuma atıyoruz.
        transform.RotateAround(Sun.transform.position, transform.up, Random.Range(1, 360));

        // Kendi etrafındaki hızı.
        rotateSelfSpeed = SolarPlanetInfo.AroundRotateSpeed * Time.deltaTime;

        // Güneşin etrafındaki hızı.
        rotateAroundSunSpeed = SolarPlanetInfo.SunAroundRotateSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Eğer gezegen bilgisi tanımlı değil ise geri dön.
        if (SolarPlanetInfo == null)
            return;

        // Kendi etrafında döndürüyoruz.
        transform.RotateAround(transform.position, transform.up, rotateSelfSpeed);

        // Eğer bu gezegen seçili ise kendi yönünün tersinde çeviriyoruz ki 
        if (PlanetZoomController.PZC.IsPlanetSelected(this))
            ZoomPanController.ZPC.MainCamera.transform.RotateAround(transform.position, -transform.up, rotateSelfSpeed);

        // Güneşin etrafında çeviriyoruz.
        if (PlanetZoomController.PZC.ZoomState == PlanetZoomController.ZoomStates.ZoomedOut)
            transform.RotateAround(Sun.transform.position, transform.up, rotateAroundSunSpeed);
    }

    private void OnMouseDown()
    {
        // Üzerine tıklandığında fokuslanıyoruz.
        if (PlanetZoomController.PZC.ZoomState == PlanetZoomController.ZoomStates.Zoomed)
            PlanetZoomController.PZC.BeginZoomOut();
        else
            PlanetZoomController.PZC.BeginZoom(this);
    }
}
