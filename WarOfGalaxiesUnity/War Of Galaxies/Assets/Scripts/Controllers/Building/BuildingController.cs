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

    [Header("Binanın ismi burada tutulacak sürekli aramak yerine.")]
    private string buildingName;

    /// <summary>
    /// Kullanıcının gezegen üzerindeki bina.
    /// </summary>
    public UserPlanetBuildingDTO UserPlanetBuilding { get; set; }

    /// <summary>
    /// Kullanıcının gezegen üzerindeki binanın yükseltmesi.
    /// </summary>
    public UserPlanetBuildingUpgDTO UserPlanetBuildingUpg { get; set; }

    IEnumerator Start()
    {
        // Binalar yüklenene kadar bekliyoruz.
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // Default gezegen seçilene kadar bekliyoruz. Yada başka bir gezegen seçilene kadar
        yield return new WaitUntil(() => GlobalPlanetController.GPC.CurrentPlanet != null);

        // Binanın ismi
        buildingName = LanguageController.LC.GetText($"B{(int)BuildingType}");

        LoadBuildingDetails();
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
                BuildingInfo.text = $"{buildingName}{Environment.NewLine}<size=2.2><color=orange>Seviye 0</color></size>";
            else
                BuildingInfo.text = $"{buildingName}{Environment.NewLine}<size=2.2><color=orange>Seviye {UserPlanetBuilding.BuildingLevel}</color></size>";

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
        GameObject buildingPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.BuildingPanel);

        // Oluşan paneldeki panel kontrol.
        BuildingPanelController bpc = buildingPanel.GetComponent<BuildingPanelController>();

        // Bina yükseltme bilgisini yüklüyoruz.
        bpc.StartCoroutine(bpc.LoadData(BuildingType));
    }

    public IEnumerator OnUpgrade()
    {
        // 1 saniye bekletiyoruz.
        yield return new WaitForSecondsRealtime(1);

        // Eğer gezegende yükseltme yok ise iptal et.
        if (UserPlanetBuildingUpg == null)
            yield break;

        // Tamamlandı mı?
        bool isCompleted = DateTime.UtcNow >= UserPlanetBuildingUpg.EndDate;

        // Eğer tarih tamamlanma tarihinnden az ise tamamlanmıştır.
        if (isCompleted)
        {
            // Kullanıcının gezegendeki kaynaklarını verify ediyoruz.
            LoginController.LC.VerifyUserResources(GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId, (UserPlanetDTO onSuccess) =>
             {
                 // Var olan binayı buluyoruz.
                 UserPlanetBuildingDTO userBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == UserPlanetBuildingUpg.UserPlanetId && x.BuildingId == UserPlanetBuildingUpg.BuildingId);

                 // Bina ilk kez kuruluyor.
                 if (userBuilding == null)
                 {
                     // Bina listesine ekliyoruz.
                     LoginController.LC.CurrentUser.UserPlanetsBuildings.Add(new UserPlanetBuildingDTO
                     {
                         BuildingId = UserPlanetBuildingUpg.BuildingId,
                         BuildingLevel = UserPlanetBuildingUpg.BuildingLevel,
                         UserPlanetId = UserPlanetBuildingUpg.UserPlanetId
                     });
                 }
                 else // Eğer bina zaten var ise sadece kaynaklarını güncelliyoruz.
                 {
                     userBuilding.BuildingLevel = UserPlanetBuildingUpg.BuildingLevel;
                 }

                 // Yükseltmeyi sistemden siliyoruz.
                 LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Remove(UserPlanetBuildingUpg);

                 // Bina detaylarını yüklüyoruz.
                 LoadBuildingDetails();

             });
        }

        // State güncelleniyor.
        updateBuildingNameLevelAndTime();

        // Sonra tekrar sayacı aktif ediyoruz.
        if (!isCompleted)
            StartCoroutine(OnUpgrade());
    }
}
