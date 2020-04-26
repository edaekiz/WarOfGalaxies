using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchItemController : MonoBehaviour
{
    // Yükseltme bilgisi.
    public UserResearchProgDTO ResearchUpgradeData { get; set; }

    [Header("Araştırma ismi buraya basılacak.")]
    public TextMeshProUGUI ResearchName;

    [Header("Araştırma ikonu buraya basılacak.")]
    public Image ResearchIcon;

    [Header("Araştırma seviyesi.")]
    public TextMeshProUGUI ResearchLevel;

    [Header("Araştırma kalan süre.")]
    public TextMeshProUGUI ResearchCountdown;

    [Header("Araştırmayı progress bar olarak göstermek için kullanacağız.")]
    public Image ResearchCountdownImage;

    [Header("Aktif araştırma")]
    public Researches CurrentResearch;

    /// <summary>
    /// Araştırma bilgilerini yükler.
    /// </summary>
    /// <param name="research"></param>
    public void LoadResearchData(Researches research)
    {
        // Aktif tuttuğu araştırma bilgisi.
        CurrentResearch = research;

        // Kullanıcının araştırmasını buluyoruz.
        UserResearchesDTO userResearch = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == research);

        // Araştırma şuanki seviyesi.
        int researchLevel = userResearch == null ? 0 : userResearch.ResearchLevel;

        // Araştırmanın ismini basıyoruz.
        ResearchName.text = research.ToString();

        // Araştırma seviyesi.
        ResearchLevel.text = researchLevel.ToString();

        // Araştırma ikonu.

        // Yükseltmesi var mı?
        UserResearchProgDTO upg = LoginController.LC.CurrentUser.UserResearchProgs.Find(x => x.ResearchID == research);

        // Eğer yükseltmesi var ise yükseltme metnini açıyoruz.
        if (upg != null)
        {
            // Eğer yükseltme var ise basıyoruz.
            ResearchUpgradeData = upg;

            // İkonu ve kalan süreyi basıyoruz.
            ResearchCountdownImage.gameObject.SetActive(true);

            // Araştırma geri sayımını aktif ediyoruz.
            ResearchCountdown.text = $"Seviye({upg.ResearchLevel}){Environment.NewLine}{TimeExtends.GetCountdownText(DateTime.UtcNow - upg.EndDate)}";
        }else
        {
            // İkonu kapatıyoruz.
            ResearchCountdownImage.gameObject.SetActive(false);
        }
    }

    public void ShowResearchDetail()
    {
        // Paneli açıyoruz.
        GameObject showedPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.ResearchDetailPanel);

        // Detaylarını buluyoruz.
        ResearchDetailItemPanel rdip = showedPanel.GetComponent<ResearchDetailItemPanel>();

        // Kullanıcının araştırmasını buluyoruz.
        UserResearchesDTO userResearch = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == CurrentResearch);

        // Ve çağırıyoruz.
        rdip.StartCoroutine(rdip.LoadReserchDetails(CurrentResearch));
    }

    private void OnResearchCompleted()
    {
        // Yükseltme bilgisi.
        UserResearchProgDTO upgradeInfo = LoginController.LC.CurrentUser.UserResearchProgs.Find(x => x.ResearchID == CurrentResearch);

        // Araştırmayı buluyoruz. Eğer yok ise eklicez var ise güncelleyeceğiz.
        UserResearchesDTO research = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == CurrentResearch);

        // Yükseltme işlemi ise seviyeyi güncelliyoruz.
        if (research != null)
            research.ResearchLevel = upgradeInfo.ResearchLevel;
        else
        {
            // Araştırmalara ekliyoruz.
            LoginController.LC.CurrentUser.UserResearches.Add(new UserResearchesDTO
            {
                ResearchID = upgradeInfo.ResearchID,
                ResearchLevel = upgradeInfo.ResearchLevel
            });
        }

        // Listeden yükseltmeyi siliyoruz.
        LoginController.LC.CurrentUser.UserResearchProgs.Remove(upgradeInfo);

        // Bütün araştırmaları yeniliyoruz.
        ResearchPanelController.RPC.RefreshAllResearches();
    }

    // Update is called once per frame
    void Update()
    {
        // Eğer yükseltme yok ise geri dön.
        if (ResearchUpgradeData == null)
            return;

        // Araştırma geri sayımını güncelliyoruz.
        ResearchCountdown.text = $"Seviye({ResearchUpgradeData.ResearchLevel}){Environment.NewLine}{TimeExtends.GetCountdownText(ResearchUpgradeData.EndDate - DateTime.UtcNow)}";

        // Araştırma biter ise doğrulaman gönderiyoruz.
        if (DateTime.UtcNow >= ResearchUpgradeData.EndDate)
        {
            // Sunucuya kaynakları doğrulamak için gönderiyoruz.
            LoginController.LC.VerifyUserResources(GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId, (UserPlanetDTO userPlanet) =>
             {
                 // Yükseltme tamalandığında.
                 OnResearchCompleted();
             });
            // Siliyoruz ki tekrar girmesin.
            ResearchUpgradeData = null;
        }
    }
}
