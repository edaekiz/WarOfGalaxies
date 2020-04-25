using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchPanelController : BasePanelController
{
    [Header("Araştırmaları yüklerken kullanılacak prefab")]
    public GameObject ResearchItem;
    [Header("Araştırmaların yükleneceği alan.")]
    public Transform ResearchContent;

    public void LoadAllResearchItems()
    {
        // Araştırmaların listesi.
        Array researches = Enum.GetValues(typeof(Researches));

        // Bütün eski araştırmaları sil.
        foreach (Transform child in ResearchContent)
            Destroy(child.gameObject);

        // Araştırmaları döneceğiz.
        for (int ii = 0; ii < researches.Length; ii++)
        {
            // Basılacak araştırma.
            Researches research = (Researches)researches.GetValue(ii);

            // Kullanıcının bu araştırma için sahip olduğu seviye.
            GameObject researchItem = Instantiate(ResearchItem, ResearchContent);

            // Kullanıcının araştırmasını buluyoruz.
            UserResearchesDTO userResearch = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == research);

            // Araştırmanın ismini basıyoruz.
            researchItem.transform.Find("ResearchName").GetComponent<TextMeshProUGUI>().text = research.ToString();

            int researchLevel = userResearch == null ? 0 : userResearch.ResearchLevel;

            // Araştırma ikonu.

            // Araştırma seviyesi.
            researchItem.transform.Find("ResearchLevelText").GetComponent<TextMeshProUGUI>().text = researchLevel.ToString();

            researchItem.GetComponent<Button>().onClick.AddListener(() => ShowResearchDetail(researchItem, research, researchLevel));

        }

    }

    
    private void ShowResearchDetail(GameObject sender, Researches research, int researchLevel)
    {
        // Paneli açıyoruz.
        GameObject showedPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.ResearchDetailPanel);

        // Detaylarını buluyoruz.
        ResearchDetailItemPanel rdip = showedPanel.GetComponent<ResearchDetailItemPanel>();

        // Ve çağırıyoruz.
        rdip.StartCoroutine(rdip.LoadReserchDetails(research, researchLevel));
    }
}
