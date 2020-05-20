using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipyardPanelController : BasePanelController
{
    /// <summary>
    /// Açık olan araştırma paneli
    /// </summary>
    public static ShipyardPanelController SPC { get; set; }

    [Header("Tersane gemilerini burada tutacağız.")]
    public List<ShipyardItemController> _shipyardItems = new List<ShipyardItemController>();

    [Header("Basılacak olan gemiler.")]
    public GameObject ShipyardItem;

    [Header("Gemileri buraya basacağız.")]
    public Transform ShipyardItemContent;

    private void Awake()
    {
        if (SPC == null)
            SPC = this;
        else
            Destroy(gameObject);
    }

    public void LoadAllShips()
    {
        // Bütün eski araştırmaları sil.
        foreach (Transform child in ShipyardItemContent)
            Destroy(child.gameObject);

        // Öncekileri temizliyoruz.
        _shipyardItems.Clear();

        // Bütün gemileri teker teker basıyoruz.
        for (int ii = 0; ii < DataController.DC.SystemData.Ships.Count; ii++)
        {
            // Gemi bilgisi.
            ShipDataDTO ship = DataController.DC.SystemData.Ships[ii];

            // Gemiyi oluşturuyoruz.
            GameObject shipyardItem = Instantiate(ShipyardItem, ShipyardItemContent);

            // Geminin controlleri.
            ShipyardItemController sic = shipyardItem.GetComponent<ShipyardItemController>();

            // Detayları yükle.
            sic.StartCoroutine(sic.LoadShipDetails((Ships)ship.ShipId));

            // Listeye ekle
            _shipyardItems.Add(sic);
        }

        // Hepsini kurduktan sonra kuyruğu yeniliyoruz.
        ShipyardQueueController.SQC.RefreshShipyardQueue();

    }

}
