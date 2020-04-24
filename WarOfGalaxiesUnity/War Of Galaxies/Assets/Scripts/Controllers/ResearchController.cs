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
        GameObject openPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.ResearchPanel);
    }

}
