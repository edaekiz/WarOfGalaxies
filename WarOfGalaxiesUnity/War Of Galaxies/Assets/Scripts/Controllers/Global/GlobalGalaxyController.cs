using Assets.Scripts.ApiModels;
using UnityEngine;

public class GlobalGalaxyController : MonoBehaviour
{
    public static GlobalGalaxyController GGC { get; set; }

    [Header("Galaksi açıldığında açılacak olanlar.")]
    public GameObject GalaxyView;

    [Header("Gezegene dönüldüğünde açılacak olanlar.")]
    public GameObject PlanetView;

    private void Awake()
    {
        if (GGC == null)
            GGC = this;
        else
            Destroy(gameObject);
    }

    public void ShowGalaxy()
    {
        GalaxyView.SetActive(true);
        PlanetView.SetActive(false);
    }

    public void CloseGalaxy()
    {
        GalaxyView.SetActive(false);
        PlanetView.SetActive(true);
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
    public bool IsInGalaxyView
    {
        get
        {
            return GalaxyView.activeSelf;
        }
    }

}
