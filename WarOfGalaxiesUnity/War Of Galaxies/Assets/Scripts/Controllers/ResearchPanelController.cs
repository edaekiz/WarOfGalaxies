using UnityEngine;

public class ResearchPanelController : BasePanelController
{
    [Header("Araştırmaları yüklerken kullanılacak prefab")]
    public GameObject ResearchItem;
    [Header("Araştırmaların yükleneceği alan.")]
    public Transform ResearchContent;
    public void LoadAllResearchItems()
    {

    }
}
