using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DefensePanelController : BasePanelController
{
    /// <summary>
    /// Açık olan defans paneli.
    /// </summary>
    public static DefensePanelController DPC { get; set; }

    [Header("Savunmaları burada tutacağız.")]
    public List<DefenseItemController> _defenseItems = new List<DefenseItemController>();

    [Header("Basılacak olan savunmalar.")]
    public GameObject DefenseItem;

    [Header("Savunmaları buraya basacağız.")]
    public Transform DefenseItemContent;

    private void Awake()
    {
        if (DPC == null)
            DPC = this;
        else
            Destroy(gameObject);
    }

    public void LoadAllDefenses()
    {
        // Bütün eski savunmaları sil.
        foreach (Transform child in DefenseItemContent)
            Destroy(child.gameObject);

        // Öncekileri temizliyoruz.
        _defenseItems.Clear();

        // Bütün savunmaları alıyoruz.
        Array defenses = Enum.GetValues(typeof(Defenses));

        // Bütün savunmaları teker teker basıyoruz.
        for (int ii = 0; ii < defenses.Length; ii++)
        {
            // Savunma bilgisi.
            Defenses defense = (Defenses)defenses.GetValue(ii);

            // Savunmayı oluşturuyoruz.
            GameObject defenseItem = Instantiate(DefenseItem, DefenseItemContent);

            // Savunma controlleri.
            DefenseItemController dic = defenseItem.GetComponent<DefenseItemController>();

            // Detayları yükle.
            dic.StartCoroutine(dic.LoadDefenseDetails(defense));

            // Listeye ekle
            _defenseItems.Add(dic);
        }

        // Hepsini kurduktan sonra kuyruğu yeniliyoruz.
        DefenseQueueController.DQC.RefreshDefenseQueue();

    }

}
