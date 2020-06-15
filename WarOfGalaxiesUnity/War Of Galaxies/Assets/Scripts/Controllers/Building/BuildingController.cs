using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BuildingController : BaseLanguageBehaviour
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
    public TMP_Text BuildingInfo;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => LoadingController.LC.IsGameLoaded);
        yield return new WaitUntil(() => GlobalPlanetController.GPC.CurrentPlanet != null);

        // Bina detaylarını yükler.
        LoadBuildingDetails();

        // Bina ismini seviye ile basıyoruz.
        InvokeRepeating("UpdateBuildingNameLevelAndTime", 0, 1);
    }

    public void LoadBuildingDetails()
    {
        // Seçim başlangıç da kalkıyor.
        SelectionMesh.SetActive(false);

        // Binayı kapatıyoruz.
        BuildingMesh.SetActive(false);

        // İnşaa edilebilir olduğunu söylüyoruz.
        ConstructableMesh.SetActive(true);

        // Kullanıcının binasını buluyoruz.
        UserPlanetBuildingDTO userPlanetBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == BuildingType);

        // Kullanıcının binası var ise açacağız binayı.
        if (userPlanetBuilding != null)
        {
            // Binanın görselini açıyoruz.
            BuildingMesh.SetActive(true);

            // İnşaa edilemez olduğunu söylüyoruz.
            ConstructableMesh.SetActive(false);
        }
    }

    public void UpdateBuildingNameLevelAndTime()
    {
        // Kullanıcının binasını buluyoruz.
        UserPlanetBuildingDTO userPlanetBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == BuildingType);

        // Yükseltme bilgisi.
        UserPlanetBuildingUpgDTO userPlanetBuildingUpg = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == BuildingType);

        // Bina ismini seviye ile basıyoruz.
        if (BuildingInfo != null)
        {
            // Texti güncelliyoruz. Bina seviyesini felan basmak üzere.     
            if (userPlanetBuilding == null)
                BuildingInfo.text = base.GetLanguageText("BinaVeSeviye", base.GetLanguageText($"B{(int)BuildingType}"), Environment.NewLine, "0");
            else
                BuildingInfo.text = base.GetLanguageText("BinaVeSeviye", base.GetLanguageText($"B{(int)BuildingType}"), Environment.NewLine, userPlanetBuilding.BuildingLevel.ToString());

            // Yükseltme var texti güncelliyoruz..
            if (userPlanetBuildingUpg != null)
                BuildingInfo.text += $"{Environment.NewLine}<color=green>{TimeExtends.GetCountdownText(userPlanetBuildingUpg.EndDate - DateTime.UtcNow)}</color>";
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
        GameObject buildingPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.BuildingPanel);

        // Oluşan paneldeki panel kontrol.
        BuildingPanelController bpc = buildingPanel.GetComponent<BuildingPanelController>();

        // Bina yükseltme bilgisini yüklüyoruz.
        bpc.StartCoroutine(bpc.LoadData(BuildingType));
    }
}
