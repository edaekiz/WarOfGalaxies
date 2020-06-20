using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResearchPanelController : BasePanelController
{
    /// <summary>
    /// Açık olan araştırma paneli
    /// </summary>
    public static ResearchPanelController RPC { get; set; }

    [Header("Araştırmaları yüklerken kullanılacak prefab")]
    public GameObject ResearchItem;

    [Header("Araştırmaların yükleneceği alan.")]
    public Transform ResearchContent;

    [Header("Araştırma binası yok ise uyarı açacağız.")]
    public TMP_Text TXT_Alert;

    // Oluşturulan araştırmaların listesi.
    private List<ResearchItemController> _researchItems = new List<ResearchItemController>();

    private void Awake()
    {
        if (RPC == null)
            RPC = this;
        else
            Destroy(gameObject);
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(CheckIsResearchBuildingExists());
    }

    public void LoadAllResearchItems()
    {
        // Bütün eski araştırmaları sil.
        foreach (Transform child in ResearchContent)
            Destroy(child.gameObject);

        // Öncekileri temizliyoruz.
        _researchItems.Clear();

        // Araştırmaları döneceğiz.
        for (int ii = 0; ii < DataController.DC.SystemData.Researches.Count; ii++)
        {
            // Basılacak araştırma.
            ResearchDataDTO research = DataController.DC.SystemData.Researches[ii];

            // Kullanıcının bu araştırma için sahip olduğu seviye.
            GameObject researchItem = Instantiate(ResearchItem, ResearchContent);

            // Araştırma datalarını yüklüyoruz.
            ResearchItemController ric = researchItem.GetComponent<ResearchItemController>();

            // Araştırma bilgisini yüklüyoruz.
            ric.StartCoroutine(ric.LoadResearchData((Researches)research.ResearchId));

            // Oluşturulan araştırmayı listeye ekliyoruz.
            _researchItems.Add(ric);
        }

    }

    /// <summary>
    /// Eğer araştırma binası yok ise uyarı açacağız.
    /// </summary>
    public IEnumerator CheckIsResearchBuildingExists()
    {
        // Giriş yapana kadar bekliyoruz.
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // Kontrol ediyoruz araştırma binası var mı?
        bool isResearchLabExists = LoginController.LC.CurrentUser.UserPlanetsBuildings.Exists(x => x.BuildingId == Buildings.ArastirmaLab);

        // Eğer araştırma yapılamıyor ise tekrar kontrol ediyoruz.
        if (!CheckLabBuilding())
        {
            // 1 saniye beklemeliyiz.
            yield return new WaitForSecondsRealtime(1);

            // Tekrar kendisini çağırıyoruz.
            StartCoroutine(CheckIsResearchBuildingExists());
        }
    }

    public bool CheckLabBuilding()
    {
       
        // Eğer araştırma lab yükseltiliyor ise bu buton açılacak.
        bool isResearchLabUpgrading = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Exists(x => x.BuildingId == Buildings.ArastirmaLab);

        // Araştırma lab yükseltiliyor ise uyarı panelini açacağız.
        if (isResearchLabUpgrading)
        {
            // Tabiki açık olmadığı durumda açıyoruz.
            TXT_Alert.text = base.GetLanguageText("AraştırmaLabYükseltmeVar");

            // Üretim yapılamaz.
            return false;
        }
        else // Yükseltilmiyor ise kapatacağız.
        {

            #region Araştırma binası yok ise araştırma yapamaz uyarı basacağız.

            // Araştırma binası var mı diye kontrol ediyoruz.
            bool isResearchBuildingExists = LoginController.LC.CurrentUser.UserPlanetsBuildings.Exists(x => x.BuildingId == Buildings.ArastirmaLab);

            // Eğer araştırma binası yok ise uyarı açacağız.
            if (!isResearchBuildingExists)
            {
                // Tabiki açık olmadığı durumda açıyoruz.
                TXT_Alert.text = base.GetLanguageText("AraştırmaLabYok");

                return false;
            }
            else
            {
                // Tabiki açık olmadığı durumda açıyoruz.
                TXT_Alert.text = string.Empty;
            }

            #endregion
        }

        // Araştırma yapabilir demek.
        return true;
    }

}
