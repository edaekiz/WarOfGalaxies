using UnityEngine;

public class GlobalGalaxyController : MonoBehaviour
{
    public static GlobalGalaxyController GGC { get; set; }

    [Header("Galaksi açıldığında açılacak olanlar.")]
    public GameObject GalaxyView;

    [Header("Gezegene dönüldüğünde açılacak olanlar.")]
    public GameObject PlanetView;

    [Header("Gezegenin etrafındaki maksimum enkaz miktarı.")]
    public int DebrisMaxQuantity;

    private void Awake()
    {
        if (GGC == null)
            GGC = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        if (GalaxyView.activeSelf)
            IsInGalaxyView = true;
        else
            IsInGalaxyView = false;
    }

    public void ShowGalaxy()
    {
        GalaxyView.SetActive(true);
        PlanetView.SetActive(false);
        IsInGalaxyView = true;
    }

    public void CloseGalaxy()
    {
        GalaxyView.SetActive(false);
        PlanetView.SetActive(true);
        IsInGalaxyView = false;
    }

    public void SwitchView()
    {
        if (GalaxyView.activeSelf)
            CloseGalaxy();
        else
            ShowGalaxy();
    }

    /// <summary>
    /// Galaksi ekranında mı?
    /// </summary>
    public bool IsInGalaxyView { get; private set; }

}
