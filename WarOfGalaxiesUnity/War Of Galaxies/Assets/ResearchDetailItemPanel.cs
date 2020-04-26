using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchDetailItemPanel : BasePanelController
{
    [Header("Araştırmanın ikonu.")]
    public Image Icon;

    [Header("Araştırmanın ismi.")]
    public TextMeshProUGUI ResearchName;

    [Header("Araştırmanın kısa açıklaması.")]
    public TextMeshProUGUI ShortDescription;

    [Header("Araştırmanın süresi.")]
    public TextMeshProUGUI UpgradeTime;

    [Header("Yükseltme butonu.")]
    public Button UpgradeButton;

    [Header("Gereken kristal miktarı nesnesini eğer kaynak 0 ise göstermeyeceğiz.")]
    public GameObject ReqMetal;

    [Header("Gereken kristal miktarı nesnesini eğer kaynak 0 ise göstermeyeceğiz.")]
    public GameObject ReqCrystal;

    [Header("Gereken boron miktarı nesnesini eğer kaynak 0 ise göstermeyeceğiz.")]
    public GameObject ReqBoron;

    [Header("Gereken metal miktarını buraya basacağız.")]
    public TextMeshProUGUI RequiredMetalQuantity;

    [Header("Gereken kristal miktarını buraya basacağız.")]
    public TextMeshProUGUI RequiredCrystalQuantity;

    [Header("Gereken boron miktarını buraya basacağız.")]
    public TextMeshProUGUI RequiredBoronQuantity;

    [Header("Aktif araştırma bilgisi")]
    public Researches CurrentResearch;

    public IEnumerator LoadReserchDetails(Researches research)
    {
        // Kullanıcının araştırması.
        UserResearchesDTO currentResearchLevel = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == research);

        // Aktif araştırmayı değiştiriyoruz.
        CurrentResearch = research;

        #region Yükseltme yapılabilir mi? Kaynak kontrolü olmadan.

        // Yükseltebilir mi?
        bool canUpgrade = true;

        // Eğer bir araştırma yükseltiliyor ise true olacak.
        bool isAlreadyUpgrading = LoginController.LC.CurrentUser.UserResearchProgs.Count > 0;

        // Eğer zaten upgrade ediliyor ise upgrade edilemez.
        if (isAlreadyUpgrading)
            canUpgrade = false;

        #endregion

        #region Araştırma Detayları.

        // Araştırma seviyesini tutuyoruz.
        int researchLevel = currentResearchLevel == null ? 0 : currentResearchLevel.ResearchLevel;

        // Araştırma ismi ve seviyesi.
        ResearchName.text = $"{research} <color=orange>({researchLevel}.Seviye)</color>";

        // Araştırma resmi.
        ShortDescription.text = research.ToString();

        // Araştırmayı buluyoruz.
        UserResearchProgDTO userResearchProg = LoginController.LC.CurrentUser.UserResearchProgs.Find(x => x.ResearchID == research);

        // Eğer seçilen araştırma yükleniyor ise süre zamanla azalacak.
        if (userResearchProg == null)
            UpgradeTime.text = TimeExtends.GetCountdownText(TimeSpan.FromSeconds(StaticData.CalculateResearchUpgradeTime(research, researchLevel)));
        else // Eğer yükseltiliyor ise kalan süreyi basıyoruz.
            UpgradeTime.text = TimeExtends.GetCountdownText(userResearchProg.EndDate - DateTime.UtcNow);

        // Maliyeti alıyoruz.
        ResourcesDTO resources = StaticData.CalculateCostResearch(research, researchLevel);

        #endregion

        #region Kaynak gereksinimlerini basıyoruz.

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

        #endregion

        #region Yükseltme kontrolü yapılıyor ise zaten yapılıyor yazacak. Aksi durumda butonu açacağız ya da kapatacağız.

        // Eğer zaten yükseltiliyor ise butonu kapat ve texti güncelle.
        if (!canUpgrade)
        {
            // Butonu kapatıyoruz.
            UpgradeButton.interactable = false;

            // Texti değiştiriyoruz.
            if (isAlreadyUpgrading)
                UpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Araştırılıyor";
            else
                UpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Araştır";
        }
        else // Eğer aksi durumdaysa yükseltme butonunu açıyoruz.
        {
            // Butonu açıyoruz.
            UpgradeButton.interactable = true;

            // Texti değiştiriyoruz.
            UpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Araştır";
        }

        #endregion

        // Bir saniye bekliyoruz.
        yield return new WaitForSecondsRealtime(1);

        // Kendisini yeniden çağırıyoruz.
        StartCoroutine(LoadReserchDetails(research));
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
                 prog.CalculateDates(prog.LeftTime);

                 // Yükseltme bilgisi.
                 LoginController.LC.CurrentUser.UserResearchProgs.Add(prog);

                 // Kullanıcının gezegeni.
                 UserPlanetDTO userPlanet = LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == prog.UserPlanetID);

                 // Gezegenin kaynaklarını yeniliyoruz.
                 userPlanet.SetPlanetResources(prog.Resources);

                 // Araştırmaları yeniliyoruz.
                 ResearchPanelController.RPC.RefreshAllResearches();
             }
         }));
    }

}
