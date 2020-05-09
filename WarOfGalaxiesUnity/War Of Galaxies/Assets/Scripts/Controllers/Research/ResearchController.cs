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


    public void ShowResearchPanel()
    {
        // Araştırma panelini açıyoruz.
        GameObject openPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.ResearchPanel);

        // Ve bütün araştırmaları yüklüyoruz.
        openPanel.GetComponent<ResearchPanelController>().LoadAllResearchItems();
    }

}
