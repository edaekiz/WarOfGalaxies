using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GlobalPanelController;

public class ResearchController : MonoBehaviour
{
    [Serializable]
    public struct ResearchWithImage
    {
        public Researches Research;
        public Sprite ResearchImage;
    }

    public static ResearchController RC { get; set; }

    private void Awake()
    {
        if (RC == null)
            RC = this;
        else
            Destroy(gameObject);
    }

    [Header("Araştırmalar ve ikonları.")]
    public List<ResearchWithImage> ResearchWithImages;

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
                OnResearchCompleted(researchProg);

                LoginController.LC.CurrentUser.UserResearchProgs.Remove(researchProg);
            }
        }

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(RecalculateResearches());
    }

    private void OnResearchCompleted(UserResearchProgDTO researchProg)
    {
        // Sunucuya kaynakları doğrulamak için gönderiyoruz.
        LoginController.LC.VerifyUserResources(GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId, (UserPlanetDTO userPlanet) =>
        {
            // Yükseltme bilgisi.
            UserResearchProgDTO upgradeInfo = LoginController.LC.CurrentUser.UserResearchProgs.Find(x => x.ResearchID == researchProg.ResearchID);

            // Araştırmayı buluyoruz. Eğer yok ise eklicez var ise güncelleyeceğiz.
            UserResearchesDTO research = LoginController.LC.CurrentUser.UserResearches.Find(x => x.ResearchID == researchProg.ResearchID);

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
        });
    }

}
