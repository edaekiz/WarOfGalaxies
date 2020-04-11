using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelController : BasePanelController
{
    [Header("Binayı yükseltme butonu.")]
    public Button UpgradeButton;

    [Header("Binayı yıkma butonu.")]
    public Button DowngradeButton;

    [Header("Binanın resmi buraya yüklenecek.")]
    public Image BuildingImage;

    [Header("Binanın ismini tutuyoruz.")]
    public TextMeshProUGUI BuildingName;

    [Header("Binanın seviyesini basıyoruz.")]
    public TextMeshProUGUI BuildingLevel;

    [Header("Binanın yükseltme süresini buraya basacağız.")]
    public TextMeshProUGUI BuildingUpgradeTime;

    [Header("Yükseltmeden sonra gerekecek enerji.")]
    public TextMeshProUGUI BuildingRequiredEnergy;

    [Header("Gereken metal miktarını buraya basacağız.")]
    public TextMeshProUGUI RequiredMetalQuantity;

    [Header("Gereken kristal miktarını buraya basacağız.")]
    public TextMeshProUGUI RequiredCrystalQuantity;

    [Header("Gereken deuterium miktarını buraya basacağız.")]
    public TextMeshProUGUI RequiredDeuteriumQuantity;

    private void OnDestroy()
    {
        // Binanın seçimini kaldırıyoruz.
        GlobalBuildingController.GBC.DeSelectBuilding();
    }

}
