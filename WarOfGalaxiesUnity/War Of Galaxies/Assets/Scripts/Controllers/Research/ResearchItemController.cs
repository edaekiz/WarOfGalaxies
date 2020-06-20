using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchItemController : BaseLanguageBehaviour
{
    [Header("Araştırma ismi buraya basılacak.")]
    public TMP_Text ResearchName;

    [Header("Araştırma ikonu buraya basılacak.")]
    public Image ResearchIcon;

    [Header("Araştırma seviyesi.")]
    public TMP_Text ResearchLevel;

    [Header("Araştırma kalan süre.")]
    public TMP_Text ResearchCountdown;

    [Header("Araştırmayı progress bar olarak göstermek için kullanacağız.")]
    public Image ResearchCountdownImage;

    [Header("Aktif araştırma")]
    public Researches CurrentResearch;

    public IEnumerator LoadResearchData(Researches research)
    {
        DateTime currentDate = DateTime.UtcNow;

        // Aktif tuttuğu araştırma bilgisi.
        CurrentResearch = research;

        // Kullanıcının araştırmasını buluyoruz.
        UserResearchesDTO userResearch = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == research);

        // Araştırma şuanki seviyesi.
        int researchLevel = userResearch == null ? 0 : userResearch.ResearchLevel;

        // Araştırmanın ismini basıyoruz.
        ResearchName.text = base.GetLanguageText($"R{(int)research}");

        // Araştırma seviyesi.
        ResearchLevel.text = $"{researchLevel}";

        // Araştırmaya atanan ikonu buluyoruz.
        ResearchImageDTO researchIcon = ResearchController.RC.ResearchWithImages.Find(x => x.Research == research);

        // Araştırma ikonu.
        if (researchIcon != null)
            ResearchIcon.sprite = researchIcon.ResearchImage;

        // Yükseltmesi var mı?
        UserResearchProgDTO upg = LoginController.LC.CurrentUser.UserResearchProgs.Find(x => x.ResearchID == research);

        // Eğer yükseltmesi var ise yükseltme metnini açıyoruz.
        if (upg != null)
        {
            // İkonu ve kalan süreyi basıyoruz.
            if (!ResearchCountdownImage.gameObject.activeSelf)
                ResearchCountdownImage.gameObject.SetActive(true);

            // Yükseltme ikonu progresini hesaplıyoruz.
            ResearchCountdownImage.fillAmount = Mathf.Clamp((float)((upg.EndDate - currentDate).TotalSeconds / (upg.EndDate - upg.BeginDate).TotalSeconds), 0, 1);

            // Araştırma geri sayımını aktif ediyoruz.
            ResearchCountdown.text = $"{base.GetLanguageText("SeviyeParantez", upg.ResearchLevel.ToString())}{Environment.NewLine}{TimeExtends.GetCountdownText(upg.EndDate - currentDate)}";
        }
        else
        {
            // İkonu kapatıyoruz.
            if (ResearchCountdownImage.gameObject.activeSelf)
                ResearchCountdownImage.gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(LoadResearchData(research));
    }

    public void ShowResearchDetail()
    {
        // Paneli açıyoruz.
        GameObject showedPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.ResearchDetailPanel);

        // Detaylarını buluyoruz.
        ResearchDetailItemPanel rdip = showedPanel.GetComponent<ResearchDetailItemPanel>();

        // Kullanıcının araştırmasını buluyoruz.
        UserResearchesDTO userResearch = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == CurrentResearch);

        // Ve çağırıyoruz.
        rdip.StartCoroutine(rdip.LoadReserchDetails(CurrentResearch));
    }

}
