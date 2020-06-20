using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchDetailItemPanel : BasePanelController
{
    [Header("Araştırmanın ikonu.")]
    public Image Icon;

    [Header("Araştırmanın ismi.")]
    public TMP_Text ResearchName;

    [Header("Araştırmanın kısa açıklaması.")]
    public TMP_Text ShortDescription;

    [Header("Araştırmanın süresi.")]
    public TMP_Text UpgradeTime;

    [Header("Yükseltme butonu.")]
    public Button UpgradeButton;

    [Header("Araştırma binası yükseltiliyor ise uyarı açacağız.")]
    public TMP_Text TXT_Alert;

    [Header("Aktif araştırma bilgisi")]
    public Researches CurrentResearch;

    protected override void Start()
    {
        base.Start();

        InvokeRepeating("RefreshState", 0, 1);
    }

    public void LoadReserchDetails(Researches research)
    {
        // Aktif araştırmayı değiştiriyoruz.
        this.CurrentResearch = research;

        // Araştırma resmi.
        ShortDescription.text = base.GetLanguageText($"RD{(int)research}");

        // Araştırma ikonu.
        ResearchImageDTO researchIcon = ResearchController.RC.ResearchWithImages.Find(x => x.Research == research);

        // Eğer araştırma ikonu var ise onu da basıyoruz.
        if (researchIcon != null)
            Icon.sprite = researchIcon.ResearchImage;
    }

    public void RefreshState()
    {
        // Kullanıcının araştırması.
        UserResearchesDTO currentResearchLevel = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == CurrentResearch);

        #region Araştırma Detayları.

        // Araştırma seviyesini tutuyoruz.
        int researchLevel = currentResearchLevel == null ? 0 : currentResearchLevel.ResearchLevel;

        // Araştırma ismi ve seviyesi.
        ResearchName.text = $"{base.GetLanguageText($"R{(int)CurrentResearch}")} <color=orange>({base.GetLanguageText("InciSeviye", researchLevel.ToString())})</color>";

        // Bir sonraki seviye.
        int nextLevel = researchLevel + 1;

        // Maliyeti alıyoruz.
        ResourcesDTO resources = DataController.DC.CalculateCostResearch(CurrentResearch, nextLevel);

        // Kaynakları set ediyoruz.
        base.SetResources(resources);

        // Toplam araştırma lab seviyesi.
        int totalResearchLevel = LoginController.LC.CurrentUser.UserPlanetsBuildings.Where(x => x.BuildingId == Buildings.ArastirmaLab).Select(x => x.BuildingLevel).DefaultIfEmpty(0).Sum();

        // Araştırmayı buluyoruz.
        UserResearchProgDTO userResearchProg = LoginController.LC.CurrentUser.UserResearchProgs.Find(x => x.ResearchID == CurrentResearch);

        // Eğer seçilen araştırma yükleniyor ise süre zamanla azalacak.
        if (userResearchProg == null)
            UpgradeTime.text = TimeExtends.GetCountdownText(TimeSpan.FromSeconds(DataController.DC.CalculateResearchUpgradeTime(resources, totalResearchLevel)));
        else // Eğer yükseltiliyor ise kalan süreyi basıyoruz.
            UpgradeTime.text = TimeExtends.GetCountdownText(userResearchProg.EndDate - DateTime.UtcNow);

        #endregion
        #region Araştırma koşulları sağlanıyor mu?

        // Yükseltebilir mi?
        bool canUpgrade = true;

        // Eğer bir araştırma yükseltiliyor ise true olacak.
        bool isAlreadyUpgrading = LoginController.LC.CurrentUser.UserResearchProgs.Count > 0;

        // Eğer zaten upgrade ediliyor ise upgrade edilemez.
        if (isAlreadyUpgrading)
            canUpgrade = false;

        #region Koşullar sağlanıyor mu?

        // Koşullar sağlanıyor mu?
        bool isCondionsTrue = TechnologyController.TC.IsInvented(TechnologyCategories.Araştırmalar, (int)CurrentResearch);

        // Eğer şartlar uygun değil ise.
        if (!isCondionsTrue)
        {
            // Butonu kapatıyoruz.
            canUpgrade = false;

            // Texti de ekrana basıyoruz.
            TXT_Alert.text = base.GetLanguageText("AraştırmaKoşulOlumsuz");
        }

        #endregion

        #region Bunu araştırabiliyor muyuz?

        // Sadece şartları yerine getirdiğimiz de kontrol ediyoruz.
        if (isCondionsTrue)
        {
            // Yükseltebiliyor muyuz diye kontrol ediyoruz.
            if (!ResearchPanelController.RPC.CheckLabBuilding())
            {
                // Yükseltemiyoruz demekki.
                canUpgrade = false;

                // Uyarıyı da buraya basacağız.
                TXT_Alert.text = ResearchPanelController.RPC.TXT_Alert.text;
            }
            else
                TXT_Alert.text = string.Empty;
        }

        #endregion
        // Eğer zaten yükseltiliyor ise butonu kapat ve texti güncelle.
        if (!canUpgrade)
        {
            // Butonu kapatıyoruz.
            UpgradeButton.interactable = false;

            // Texti değiştiriyoruz.
            if (isAlreadyUpgrading)
                UpgradeButton.GetComponentInChildren<TMP_Text>().text = base.GetLanguageText("Araştırılıyor");
            else
                UpgradeButton.GetComponentInChildren<TMP_Text>().text = base.GetLanguageText("Araştır");
        }
        else // Eğer aksi durumdaysa yükseltme butonunu açıyoruz.
        {
            // Butonu açıyoruz.
            UpgradeButton.interactable = true;

            // Texti değiştiriyoruz.
            UpgradeButton.GetComponentInChildren<TMP_Text>().text = base.GetLanguageText("Araştır");
        }

        #endregion
    }

    public void UpgradeResearch()
    {
        StartCoroutine(ApiService.API.Post("UpgradeUserResearch", new UserResearchUpgRequest
        {
            ResearchID = CurrentResearch,
            UserPlanetID = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId
        }, (ApiResult response) =>
         {

             // Eğer başarılı ise.
             if (response.IsSuccess)
             {
                 // Yükseltme modelini alıyoruz.
                 UserResearchProgDTO prog = response.GetData<UserResearchProgDTO>();

                 // Hesaplamasını yapıyoruz.
                 prog.CalculateDates(prog.PassedTime, prog.LeftTime);

                 // Yükseltme bilgisi.
                 LoginController.LC.CurrentUser.UserResearchProgs.Add(prog);

                 // Kullanıcının gezegeni.
                 UserPlanetDTO userPlanet = LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == prog.UserPlanetID);

                 // Gezegenin kaynaklarını yeniliyoruz.
                 userPlanet.SetPlanetResources(prog.Resources);

                 // Yükselt panelini kapatıyoruz. 
                 base.ClosePanel();
             }
         }));
    }

    public void ShowConditions() => TechnologyController.TC.ShowTechnologyPanelWithItem(TechnologyCategories.Araştırmalar, (int)CurrentResearch);

}
