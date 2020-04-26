using Assets.Scripts.Enums;
using System;
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
            Destroy(RPC.gameObject);
    }

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

            // Araştırma datalarını yüklüyoruz.
            ResearchItemController ric = researchItem.GetComponent<ResearchItemController>();

            // Araştırma bilgisini yüklüyoruz.
            ric.LoadResearchData(research);

            // Oluşturulan araştırmayı listeye ekliyoruz.
            _researchItems.Add(ric);
        }
    }

    /// <summary>
    /// Bütün araştırmaları yeniliyoruz.
    /// </summary>
    public void RefreshAllResearches() => _researchItems.ForEach(e => e.LoadResearchData(e.CurrentResearch));

}
