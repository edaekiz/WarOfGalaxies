using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
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
        yield return new WaitUntil(() => GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId > 0);

        // Kullanıcının binasını buluyoruz.
        UserPlanetBuildingDTO userBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == BuildingType);

        // Kullanıcının binası var ise açacağız binayı.
        if (userBuilding != null)
        {
            // Binanın görselini açıyoruz.
            BuildingMesh.SetActive(true);

            // İnşaa edilemez olduğunu söylüyoruz.
            ConstructableMesh.SetActive(false);

            // Bina ismini seviye ile basıyoruz.
            if (BuildingInfo != null)
                BuildingInfo.text = $"{BuildingType.ToString()}{Environment.NewLine}<size=2.2><color=orange>Seviye {userBuilding.BuildingLevel}</color></size>";
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
}
