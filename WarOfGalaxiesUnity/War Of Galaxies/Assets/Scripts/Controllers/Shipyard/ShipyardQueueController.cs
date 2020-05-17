﻿using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipyardQueueController : BaseLanguageBehaviour
{
    public static ShipyardQueueController SQC { get; set; }

    private void Awake()
    {
        if (SQC == null)
            SQC = this;
        else
            Destroy(gameObject);
    }

    [Header("Kuyruk oluştuğunda eklenecek olan eşya.")]
    public GameObject ShipyardQueueItem;

    [Header("Tersane kuyruğunu buraya yükleyeceğiz.")]
    public Transform ContentField;

    public void RefreshShipyardQueue()
    {
        // Eskilerini siliyoruz.
        foreach (Transform child in ContentField)
            Destroy(child.gameObject);

        // Şimdi yenilerini basıyoruz.
        foreach (UserPlanetShipProgDTO queue in LoginController.LC.CurrentUser.UserPlanetShipProgs)
        {
            // Kuyruk eşyası.
            GameObject go = Instantiate(ShipyardQueueItem, ContentField);

            // Resmi yüklüyoruz.
            go.transform.Find("ItemImage").GetComponent<Image>().sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == queue.ShipId).ShipImage;

            // İsmini basıyoruz.
            go.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"S{queue.ShipId}");

            // Miktarı basıyoruz.
            go.transform.Find("ItemCount").GetComponent<TMP_Text>().text = queue.ShipCount.ToString();
        }
    }

}
