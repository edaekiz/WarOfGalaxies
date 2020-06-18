﻿using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using System.Linq;
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

    [Header("Eğer yükseltme yapılamaz ise buradaki uyarı açılacak")]
    public GameObject TXT_Alert;

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
        ItemNameWithQuantity.text = $"{base.GetLanguageText($"S{(int)ship}")} <color=orange>({quantity})</color>";

        // Açıklamasını basıyoruz.
        ItemDescription.text = base.GetLanguageText($"SD{(int)ship}");

        // Geminin resmini basıyoruz.
        ItemImage.sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == ship).ShipImage;

        // Maliyeti.
        ShipDataDTO shipInfo = DataController.DC.GetShip(ship);

        // Eğer üretim yapılabiliyor ise true olacak.
        bool canProduce = true;

        // Eğer gönderim var ise butonu kapatıyoruz.
        if (IsSending)
            canProduce = false;

        // Maliyeti set ediyoruz.
        bool isMathEnough = base.SetResources(new ResourcesDTO(shipInfo.CostMetal, shipInfo.CostCrystal, shipInfo.CostBoron));

        // Eğer materyal yeterli ise butonu açıyoruz değil ise kapatıyoruz.
        if (!isMathEnough)
            canProduce = false;

        // Tersaneyi buluyoruz.
        UserPlanetBuildingDTO shipyard = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.BuildingId == Buildings.Tersane && x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Üretim süresini basıyoruz.
        double countdown = DataController.DC.CalculateShipCountdown(ship, shipyard == null ? 0 : shipyard.BuildingLevel);

        // Üretim süresini basıyoruz.
        ItemCountdown.text = TimeExtends.GetCountdownText(TimeSpan.FromSeconds(countdown));

        #region Tersane yükseltiliyor ise gemi üretilemez.

        // Eğer tersane yükseltiliyor ise bu buton açılacak.
        bool isTersaneUpgrading = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

        // Tersane yükseltiliyor ise uyarı panelini açacağız.
        if (isTersaneUpgrading)
        {
            // Tabiki açık olmadığı durumda açıyoruz.
            if (!TXT_Alert.activeSelf)
                TXT_Alert.SetActive(true);

            // Üretim yapılamaz.
            canProduce = false;
        }else
        {
            // Tabiki açık olmadığı durumda açıyoruz.
            if (TXT_Alert.activeSelf)
                TXT_Alert.SetActive(false);
        }

        #endregion

        // Üretim yapılabiliyor ise butonu açıyoruz.
        if (canProduce)
            ProduceButton.interactable = true;
        else
            ProduceButton.interactable = false;

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
                    if (LoginController.LC.CurrentUser.UserPlanetShipProgs.Where(x => x.UserPlanetId == responseData.UserPlanetID).Count() == 0)
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
