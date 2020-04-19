using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using System.Linq;
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

    [Header("Gereken boron miktarını buraya basacağız.")]
    public TextMeshProUGUI RequiredBoronQuantity;

    public IEnumerator LoadData(Buildings building)
    {
        // Yükseltebilir mi?
        bool canUpgrade = true;

        // Eğer bir bina yükseltiliyor ise true olacak.
        bool isAlreadyUpgrading = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Count(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId) > 0;

        // Eğer zatne upgrade ediliyor ise upgrade edilemez.
        if (isAlreadyUpgrading)
            canUpgrade = false;

        // Kullanıcının binassı.
        UserPlanetBuildingDTO userBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == building);

        // Kullanıcının binası yok ise 0. kademe yazacak var ise tesisin seviyesini yazacak.
        if (userBuilding == null)
            BuildingLevel.text = $"Kademe <color=#C4E5FD>{0}</color>";
        else
            BuildingLevel.text = $"Kademe <color=#C4E5FD>{userBuilding.BuildingLevel}</color>";

        int nextLevel = userBuilding == null ? 1 : userBuilding.BuildingLevel + 1;

        // Binanın ismini basıyoruz.
        BuildingName.text = building.ToString();

        // Robot fabrikasını buluyoruz.
        UserPlanetBuildingDTO robotBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

        // Yükseltme süresi.
        double upgradeTime = StaticData.CalculateBuildingUpgradeTime(building, nextLevel, robotBuilding == null ? 0 : robotBuilding.BuildingLevel);

        // Ekrana basıyoruz.
        BuildingUpgradeTime.text = $"<color=#C4E5FD>Yapım Süresi :</color> {TimeExtends.GetCountdownText(TimeSpan.FromSeconds(upgradeTime))}";

        // Kaynak kontrolü ve koşulları sağlıyor mu kontorlü
        ResourcesDTO resources = StaticData.CalculateCostBuilding(building, nextLevel);

        // Gereken metal kaynağı.
        RequiredMetalQuantity.text = ResourceExtends.ConvertResource(resources.Metal);

        // Eğer gereken kaynak kadar kaynağı yok ise gezegenin kırmızı yanacak.
        if (resources.Metal > GlobalPlanetController.GPC.CurrentPlanet.Metal)
        {
            // Metal madenini kırmzııya boyuyoruz.
            RequiredMetalQuantity.color = Color.red;

            // Eğer yetersiz ise bu bina yükseltilemez.
            canUpgrade = false;
        }
        else
            RequiredMetalQuantity.color = Color.white;

        // Gereken kristal kaynağı.
        RequiredCrystalQuantity.text = ResourceExtends.ConvertResource(resources.Crystal);

        // Eğer gereken kaynak kadar kaynağı yok ise gezegenin kırmızı yanacak.
        if (resources.Crystal > GlobalPlanetController.GPC.CurrentPlanet.Crystal)
        {
            // Kristal madenini kırmzııya boyuyoruz.
            RequiredCrystalQuantity.color = Color.red;

            // Eğer yetersiz ise bu bina yükseltilemez.
            canUpgrade = false;
        }
        else
            RequiredCrystalQuantity.color = Color.white;

        // Gereken boron kaynağı.
        RequiredBoronQuantity.text = ResourceExtends.ConvertResource(resources.Boron);

        // Eğer gereken kaynak kadar kaynağı yok ise gezegenin kırmızı yanacak.
        if (resources.Boron > GlobalPlanetController.GPC.CurrentPlanet.Boron)
        {
            // Bor madenini kırmzııya boyuyoruz.
            RequiredBoronQuantity.color = Color.red;

            // Eğer yetersiz ise bu bina yükseltilemez.
            canUpgrade = false;
        }
        else
            RequiredBoronQuantity.color = Color.white;


        // Eğer zaten yükseltiliyor ise butonu kapat ve texti güncelle.
        if (!canUpgrade)
        {
            // Butonu kapatıyoruz.
            UpgradeButton.interactable = false;

            // Texti değiştiriyoruz.
            if (isAlreadyUpgrading)
                UpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Yükseltiliyor";
            else
                UpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Yükselt";
        }
        else // Eğer aksi durumdaysa yükseltme butonunu açıyoruz.
        {
            // Butonu açıyoruz.
            UpgradeButton.interactable = true;

            // Texti değiştiriyoruz.
            UpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Yükselt";
        }

        // 1 saniye bekliyoruz sonra kendisini yeniden çağıracağız.
        yield return new WaitForSecondsRealtime(1);

        // Tekrar çağırıyoruz.
        StartCoroutine(LoadData(building));
    }

    private void OnDestroy()
    {
        // Binanın seçimini kaldırıyoruz.
        GlobalBuildingController.GBC.DeSelectBuilding();

        // Bütün timerları durduruyoruz bu sınıftaki.
        StopAllCoroutines();
    }

}
