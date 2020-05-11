﻿using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipyardDetailItemPanel : BasePanelController
{
    [Header("Geminin ismi ile miktarı.")]
    public TMP_Text ItemNameWithQuantity;

    [Header("Geminin kısa açıklaması.")]
    public TMP_Text ItemDescription;

    [Header("Üretim süresi.")]
    public TMP_Text ItemCountdown;

    [Header("Üretim butonu")]
    public Button ProduceButton;

    [Header("Geminin ikonu.")]
    public Image ItemImage;

    [Header("Aktif gemi bilgisi")]
    public Ships CurrentShip;

    [Header("Üretilecek miktar")]
    public TMP_InputField QuantityField;

    [Header("Sunucuya istek gönderiliyor mu?")]
    public bool IsSending;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public IEnumerator LoadShipDetals(Ships ship)
    {
        // Gemi bilgisi.
        CurrentShip = ship;

        // Aktif gemi miktarı.
        UserPlanetShipDTO currentShipCount = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == ship);

        // Gemiye sahip ise miktarı değil ise 0.
        string quantity = currentShipCount == null ? "0" : currentShipCount.ShipCount.ToString();

        // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
        ItemNameWithQuantity.text = $"{base.GetLanguageText($"G{(int)ship}")} <color=orange>({quantity})</color>";

        // Açıklamasını basıyoruz.
        ItemDescription.text = base.GetLanguageText($"GD{(int)ship}");

        // Geminin resmini basıyoruz.
        ItemImage.sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == ship).ShipImage;

        // Maliyeti.
        ShipDTO shipInfo = StaticData.ShipData.Find(x => x.ShipID == ship);

        // Maliyeti set ediyoruz.
        bool isMathEnough = base.SetResources(shipInfo.Cost);

        // Eğer materyal yeterli ise butonu açıyoruz değil ise kapatıyoruz.
        if (isMathEnough && !IsSending)
            ProduceButton.interactable = true;
        else
            ProduceButton.interactable = false;

        // Tersaneyi buluyoruz.
        UserPlanetBuildingDTO shipyard = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.BuildingId == Buildings.Tersane && x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Üretim süresini basıyoruz.
        double countdown = StaticData.CalculateShipCountdown(ship, shipyard == null ? 0 : shipyard.BuildingLevel);

        // Üretim süresini basıyoruz.
        ItemCountdown.text = TimeExtends.GetCountdownText(TimeSpan.FromSeconds(countdown));

        // 1 saniye bekliyoruz tekrar yenileyeceğiz.
        yield return new WaitForSecondsRealtime(1);

        // Gemi bilgisini yüklüyoruz.
        StartCoroutine(LoadShipDetals(ship));
    }

    public void AddToQueue()
    {
        string quantityStr = QuantityField.text;
        int quantity = 0;

        if (int.TryParse(quantityStr, out quantity))
        {
            // Gönderiliyor mu?
            IsSending = true;

            // Butonu kapatıyoruz.
            ProduceButton.interactable = false;

            StartCoroutine(ApiService.API.Post("AddShipToShipyardQueue", new ShipyardAddQueueRequestDTO
            {
                Quantity = quantity,
                ShipID = CurrentShip,
                UserPlanetID = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId
            }, (ApiResult response) =>
            {
                // Eğer başarılı ise.
                if (response.IsSuccess)
                {
                    // Gelen datayı alıyoruz.
                    ShipyardAddQueueResponseDTO responseData = response.GetData<ShipyardAddQueueResponseDTO>();

                    UserPlanetShipProgDTO progress = new UserPlanetShipProgDTO
                    {
                        ShipCount = responseData.Quantity,
                        ShipId = responseData.ShipID,
                        UserPlanetId = responseData.UserPlanetID
                    };

                    // Eğer üretim yok ise tarih veriyoruz.
                    if (LoginController.LC.CurrentUser.UserPlanetShipProgs.Count == 0)
                        progress.LastVerifyDate = DateTime.UtcNow;

                    // Üretime ekliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetShipProgs.Add(progress);

                    // Kaynakları güncelliyoruz.
                    LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == responseData.UserPlanetID).SetPlanetResources(responseData.PlanetResources);

                    // Paneli yeniliyoruz.
                    ShipyardQueueController.SQC.RefreshShipyardQueue();

                    // Detay panelini kapatıyoruz.
                    base.ClosePanel();
                }

            }));
        }
    }

}
