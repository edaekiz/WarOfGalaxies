using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPanelController : BasePanelController
{
    /// <summary>
    /// Açık olan araştırma paneli
    /// </summary>
    public static ResearchPanelController RPC { get; set; }

    [Header("Araştırmaları yüklerken kullanılacak prefab")]
    public GameObject ResearchItem;

    [Header("Araştırmaların yükleneceği alan.")]
    public Transform ResearchContent;

    // Oluşturulan araştırmaların listesi.
    private List<ResearchItemController> _researchItems = new List<ResearchItemController>();

    private void Awake()
    {
        if (RPC == null)
            RPC = this;
        else
            Destroy(gameObject);
    }

    public void LoadAllResearchItems()
    {
        // Bütün eski araştırmaları sil.
        foreach (Transform child in ResearchContent)
            Destroy(child.gameObject);

        _researchItems.Clear();

        // Araştırmaları döneceğiz.
        for (int ii = 0; ii < DataController.DC.SystemData.Researches.Count; ii++)
        {
            // Basılacak araştırma.
            ResearchDataDTO research = DataController.DC.SystemData.Researches[ii];

            // Kullanıcının bu araştırma için sahip olduğu seviye.
            GameObject researchItem = Instantiate(ResearchItem, ResearchContent);

            // Araştırma datalarını yüklüyoruz.
            ResearchItemController ric = researchItem.GetComponent<ResearchItemController>();

            // Araştırma bilgisini yüklüyoruz.
            ric.StartCoroutine(ric.LoadResearchData((Researches)research.ResearchId));

            // Oluşturulan araştırmayı listeye ekliyoruz.
            _researchItems.Add(ric);
        }
    }

}
