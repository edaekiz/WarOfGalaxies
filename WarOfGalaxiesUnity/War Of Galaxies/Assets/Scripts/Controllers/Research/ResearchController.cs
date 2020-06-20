using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GlobalPanelController;

public class ResearchController : MonoBehaviour
{
    public static ResearchController RC { get; set; }

    private void Awake()
    {
        if (RC == null)
            RC = this;
        else
            Destroy(gameObject);
    }
    [Header("Araştırma yapılırken bu obje aktif edilecek.")]
    public GameObject ResearchProgressIcon;

    [Header("Araştırmalar ve ikonları.")]
    public List<ResearchImageDTO> ResearchWithImages;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => LoadingController.LC.IsGameLoaded);
        StartCoroutine(RecalculateResearches());
    }

    public void ShowResearchPanel()
    {
        // Araştırma panelini açıyoruz.
        GameObject openPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.ResearchPanel);

        // Ve bütün araştırmaları yüklüyoruz.
        openPanel.GetComponent<ResearchPanelController>().LoadAllResearchItems();
    }

    public IEnumerator RecalculateResearches()
    {
        UserResearchProgDTO researchProg = LoginController.LC.CurrentUser.UserResearchProgs.FirstOrDefault();

        if (researchProg != null)
        {
            DateTime currentDate = DateTime.UtcNow;
            if (currentDate >= researchProg.EndDate)
            {
                // Devam eden araştırmayı siliyoruz.
                LoginController.LC.CurrentUser.UserResearchProgs.Remove(researchProg);

                // Araştırma tamamlandı deyip verify ediyoruz.
                OnResearchCompleted(researchProg);
            }
        }
        
        // Her saniye sonunda gerekiyorsa progress ikonunu açacağız ya da kapatacağız.
        RefreshProgressIcon();

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(RecalculateResearches());
    }

    private void OnResearchCompleted(UserResearchProgDTO researchProg)
    {
        // Sunucuya kaynakları doğrulamak için gönderiyoruz.
        LoginController.LC.VerifyUserResources(GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId, (UserPlanetDTO userPlanet) =>
        {
            // Araştırmayı buluyoruz. Eğer yok ise eklicez var ise güncelleyeceğiz.
            UserResearchesDTO research = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == researchProg.ResearchID);

            // Yükseltme işlemi ise seviyeyi güncelliyoruz.
            if (research != null)
                research.ResearchLevel = researchProg.ResearchLevel;
            else
            {
                // Araştırmalara ekliyoruz.
                LoginController.LC.CurrentUser.UserResearches.Add(new UserResearchesDTO
                {
                    ResearchID = researchProg.ResearchID,
                    ResearchLevel = researchProg.ResearchLevel
                });
            }

            // Araştırmaları tekrar yüklüyoruz.
            if (ResearchPanelController.RPC != null)
                ResearchPanelController.RPC.LoadAllResearchItems();

        });
    }

    public void RefreshProgressIcon()
    {
        if (LoginController.LC.CurrentUser.UserResearchProgs.Count > 0)
        {
            if (!ResearchProgressIcon.activeSelf)
                ResearchProgressIcon.SetActive(true);
        }
        else
        {
            if (ResearchProgressIcon.activeSelf)
                ResearchProgressIcon.SetActive(false);
        }
    }

}
