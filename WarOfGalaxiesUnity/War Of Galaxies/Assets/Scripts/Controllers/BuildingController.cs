using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [Header("Seçim yapıldığında açılacak olan mesh.")]
    public GameObject SelectionMesh;

    [Header("Binanın kurulu olduğunda basılacak olan meshi.")]
    public GameObject BuildingMesh;

    [Header("Binaya sahip olmadığında görüntülenecek olan mesh.")]
    public GameObject ConstructableMesh;

    [Header("Veritabanında karşılık gelen bina.")]
    public Buildings BuildingType;

    [Header("Bina ismini ve seviyesini basacağız.")]
    public TextMeshProUGUI BuildingInfo;

    /// <summary>
    /// Kullanıcının gezegen üzerindeki bina.
    /// </summary>
    public UserPlanetBuildingDTO UserPlanetBuilding { get; set; }

    /// <summary>
    /// Kullanıcının gezegen üzerindeki binanın yükseltmesi.
    /// </summary>
    public UserPlanetBuildingUpgDTO UserPlanetBuildingUpg { get; set; }

    private void Start()
    {
        StartCoroutine(LoadBuildingDetails());
    }

    private IEnumerator LoadBuildingDetails()
    {
        // Seçim başlangıç da kalkıyor.
        SelectionMesh.SetActive(false);

        // Binayı kapatıyoruz.
        BuildingMesh.SetActive(false);

        // İnşaa edilebilir olduğunu söylüyoruz.
        ConstructableMesh.SetActive(true);

        // Binalar yüklenene kadar bekliyoruz.
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // Default gezegen seçilene kadar bekliyoruz. Yada başka bir gezegen seçilene kadar
        yield return new WaitUntil(() => GlobalPlanetController.GPC.CurrentPlanet != null);

        // Kullanıcının binasını buluyoruz.
        UserPlanetBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == BuildingType);

        // Kullanıcının binası var ise açacağız binayı.
        if (UserPlanetBuilding != null)
        {
            // Binanın görselini açıyoruz.
            BuildingMesh.SetActive(true);

            // İnşaa edilemez olduğunu söylüyoruz.
            ConstructableMesh.SetActive(false);
        }

        // Yükseltme için arıyoruz.
        UserPlanetBuildingUpg = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == BuildingType);

        // Eğer yükseltme var ise yükseltmeyi başlatıyoruz. 
        if (UserPlanetBuildingUpg != null)
        {
            // Başlangıç ve bitiş tarihini ayarlıyoruz.
            UserPlanetBuildingUpg.CalculateDates();

            // Sayacı açıyoruz.
            StartCoroutine(OnUpgrade());
        }

        // Bina ismini seviye ile basıyoruz.
        updateBuildingNameLevelAndTime();

    }

    private void updateBuildingNameLevelAndTime()
    {
        // Bina ismini seviye ile basıyoruz.
        if (BuildingInfo != null)
        {
            // Texti güncelliyoruz. Bina seviyesini felan basmak üzere.     
            if (UserPlanetBuilding == null)
                BuildingInfo.text = $"{BuildingType.ToString()}{Environment.NewLine}<size=2.2><color=orange>Seviye 0</color></size>";
            else
                BuildingInfo.text = $"{BuildingType.ToString()}{Environment.NewLine}<size=2.2><color=orange>Seviye {UserPlanetBuilding.BuildingLevel}</color></size>";

            // Yükseltme var texti güncelliyoruz..
            if (UserPlanetBuildingUpg != null)
                BuildingInfo.text += $"{Environment.NewLine}<color=green>{TimeExtends.GetCountdownText(UserPlanetBuildingUpg.EndDate - DateTime.UtcNow)}</color>";
        }
    }

    private void OnMouseDown()
    {
        // Eğer zaten bir panel açık ise geri dön.
        if (GlobalPanelController.GPC.IsAnyPanelOpen)
            return;

        // Binayı seçiyoruz.
        GlobalBuildingController.GBC.SelectBuilding(this);

        // Paneli gösteriyoruz.
        GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.BuildingPanel);
    }

    public IEnumerator OnUpgrade()
    {
        // 1 saniye bekletiyoruz.
        yield return new WaitForSecondsRealtime(1);

        // State güncelleniyor.
        updateBuildingNameLevelAndTime();

        // Sonra tekrar sayacı aktif ediyoruz.
        if (UserPlanetBuildingUpg != null)
            StartCoroutine(OnUpgrade());
    }

}
