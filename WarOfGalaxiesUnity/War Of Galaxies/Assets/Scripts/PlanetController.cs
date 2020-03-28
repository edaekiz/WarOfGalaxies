using Assets.Scripts.Models;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [Header("Yörüngesindeki güneş")]
    public GameObject Sun;

    [Header("Gösterdiği gezegen bilgisi.")]
    public SolarPlanetDTO SolarPlanetInfo;

    public void LoadPlanetInfo(GameObject sun, SolarPlanetDTO solarPlanet)
    {
        // Güneşi atıyoruz.
        Sun = sun;

        // Gezegen bilgisini atıyoruz.
        SolarPlanetInfo = solarPlanet;

        // Güneşin etrafında rastgele bir konuma atıyoruz.
        transform.RotateAround(Sun.transform.position, transform.up, Random.Range(1, 360));
    }

    // Update is called once per frame
    void Update()
    {
        // Eğer gezegen bilgisi tanımlı değil ise geri dön.
        if (SolarPlanetInfo == null)
            return;

        // Kendi etrafında döndürüyoruz.
        transform.RotateAround(transform.position, transform.up, SolarPlanetInfo.AroundRotateSpeed * Time.deltaTime);

        // Güneşin etrafında rastgele bir konuma atıyoruz.
        transform.RotateAround(Sun.transform.position, transform.up, SolarPlanetInfo.SunAroundRotateSpeed * Time.deltaTime);
    }

    private void OnMouseDown()
    {
        // Üzerine tıklandığında fokuslanıyoruz.
        PlanetZoomController.PZC.BeginZoom(this.transform);
    }
}
