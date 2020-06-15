using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
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
    public TMP_Text ResearchName;

    [Header("Araştırmanın kısa açıklaması.")]
    public TMP_Text ShortDescription;

    [Header("Araştırmanın süresi.")]
    public TMP_Text UpgradeTime;

    [Header("Yükseltme butonu.")]
    public Button UpgradeButton;

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
        ResearchName.text = $"{base.GetLanguageText($"R{(int)research}")} <color=orange>({base.GetLanguageText("InciSeviye", researchLevel.ToString())})</color>";

        // Bir sonraki seviye.
        int nextLevel = researchLevel + 1;

        // Araştırma resmi.
        ShortDescription.text = base.GetLanguageText($"RD{(int)research}");

        // Araştırma ikonu.
        ResearchImageDTO researchIcon = ResearchController.RC.ResearchWithImages.Find(x => x.Research == research);

        // Eğer araştırma ikonu var ise onu da basıyoruz.
        if (researchIcon != null)
            Icon.sprite = researchIcon.ResearchImage;

        // Araştırmayı buluyoruz.
        UserResearchProgDTO userResearchProg = LoginController.LC.CurrentUser.UserResearchProgs.Find(x => x.ResearchID == research);

        // Eğer seçilen araştırma yükleniyor ise süre zamanla azalacak.
        if (userResearchProg == null)
            UpgradeTime.text = TimeExtends.GetCountdownText(TimeSpan.FromSeconds(DataController.DC.CalculateResearchUpgradeTime(research, nextLevel)));
        else // Eğer yükseltiliyor ise kalan süreyi basıyoruz.
            UpgradeTime.text = TimeExtends.GetCountdownText(userResearchProg.EndDate - DateTime.UtcNow);

        // Maliyeti alıyoruz.
        ResourcesDTO resources = DataController.DC.CalculateCostResearch(research, nextLevel);

        #endregion

        // Kaynakları set ediyoruz.
        base.SetResources(resources);

        #region Yükseltme kontrolü yapılıyor ise zaten yapılıyor yazacak. Aksi durumda butonu açacağız ya da kapatacağız.

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
                 prog.CalculateDates(prog.PassedTime, prog.LeftTime);

                 // Yükseltme bilgisi.
                 LoginController.LC.CurrentUser.UserResearchProgs.Add(prog);

                 // Kullanıcının gezegeni.
                 UserPlanetDTO userPlanet = LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == prog.UserPlanetID);

                 // Gezegenin kaynaklarını yeniliyoruz.
                 userPlanet.SetPlanetResources(prog.Resources);

             }
         }));
    }

}
