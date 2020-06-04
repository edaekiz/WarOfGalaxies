using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanetActionFooterPanel : BasePanelController
{
    [Header("Gezegen ve kordinat ismini basacağımız alan.")]
    public TMP_Text TXT_PlanetNameWithCordinate;

    [Header("Hızlı casus gemisi göndermek için button.")]
    public Button FastSpyButton;

    [Header("Hızlı şekilde gönderilecek olan casusluk gemisi miktarı.")]
    public TMP_Text TXT_FastSpyQuantity;

    [Header("Gösterilecek olan panel.")]
    public GameObject GarbagePanel;

    [Header("Toplanabilir metal miktarı.")]
    public TMP_Text TXT_GarbageMetalQuantity;

    [Header("Toplanabilir kristal miktarı.")]
    public TMP_Text TXT_GarbageCrystalQuantity;

    [Header("Toplanabilir bor miktarı.")]
    public TMP_Text TXT_GarbageBoronQuantity;

    /// <summary>
    /// Gösterilen kordinata ait bilgiler.
    /// </summary>
    public CordinateDTO CurrentShownCordinate { get; set; }

    /// <summary>
    /// Gösterilen gezegen bilgisi.
    /// </summary>
    public SolarPlanetDTO CurrentShownPlanet { get; set; }

    protected override void Update()
    {
        base.Update();

        // Boş alana tıklandığında paneli kapatıyoruz.
        if (Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null)
            base.ClosePanel();
    }

    public void ShowCordinate(SolarPlanetDTO solarPlanet, CordinateDTO cordinate)
    {
        // Gösterilen kordinat bilgisi.
        this.CurrentShownCordinate = cordinate;

        // Gösterilen gezegen bilgisi.
        this.CurrentShownPlanet = solarPlanet;

        // Gezegenin adını ve kordinatını basıyoruz.
        if (solarPlanet == null)
            TXT_PlanetNameWithCordinate.text = base.GetLanguageText("KeşfedilmemişGezegen");
        else if (solarPlanet.UserPlanet != null)
            TXT_PlanetNameWithCordinate.text = base.GetLanguageText(solarPlanet.UserPlanet.PlanetName);

        // Kordinatı basıyoruz.
        TXT_PlanetNameWithCordinate.text += $" ({CordinateExtends.ToCordinateString(cordinate)})";

        // Eğer enkaz varsa paneli açıyoruz.
        ShowGarbagePanelIfRequired();

        // Butonu kontrol ediyoruz.
        ValidateSpyShipCount();
    }

    private void ShowGarbagePanelIfRequired()
    {
        // Eğer enkaz var ise gösteriyoruz.
        if (this.CurrentShownPlanet.GarbageMetal > 0 || this.CurrentShownPlanet.GarbageCrystal > 0 || this.CurrentShownPlanet.GarbageBoron > 0)
            GarbagePanel.SetActive(true);
        else
        {
            // Paneli kapatıyoruz.
            if (GarbagePanel.activeSelf)
                GarbagePanel.SetActive(false);

            // Devamına inmeye gerek yok.
            return;
        }

        // Toplanabilir metal miktarını basıyoruz.
        TXT_GarbageMetalQuantity.text = ResourceExtends.ConvertToDottedResource(this.CurrentShownPlanet.GarbageMetal);

        // Toplanabilir kristal miktarını basıyoruz.
        TXT_GarbageCrystalQuantity.text = ResourceExtends.ConvertToDottedResource(this.CurrentShownPlanet.GarbageCrystal);

        // Toplanabilir bor miktarını basıyoruz.
        TXT_GarbageBoronQuantity.text = ResourceExtends.ConvertToDottedResource(this.CurrentShownPlanet.GarbageBoron);



    }

    public void ClickAllAction()
    {
        // Paneli açıyoruz.
        GameObject gpap = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.GalaxyPlanetActionPanel);

        // Panele dataları yüklüyoruz.
        gpap.GetComponent<PlanetActionController>().Load(this.CurrentShownPlanet, this.CurrentShownCordinate);
    }

    public void FastSpy()
    {
        int spyCount = int.Parse(TXT_FastSpyQuantity.text);

        // Gönderilecek olan gemiler. Sadece casusluk.
        List<Tuple<Ships, int>> spyShip = new List<Tuple<Ships, int>> { new Tuple<Ships, int>(Ships.CasusSondası, spyCount) };

        // Gönderilecek gemi datasını oluşturuyoruz.
        string shipData = FleetExtends.ShipDataToStringData(spyShip);

        // Uçuş bilgileri.
        SendFleetFromPlanetDTO requestData = new SendFleetFromPlanetDTO
        {
            SenderUserPlanetId = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId,
            CarriedBoron = 0,
            CarriedCrystal = 0,
            CarriedMetal = 0,
            FleetSpeed = 1,
            FleetType = (int)FleetTypes.Casusluk,
            DestinationGalaxyIndex = CurrentShownCordinate.GalaxyIndex,
            DestinationSolarIndex = CurrentShownCordinate.SolarIndex,
            DestinationOrderIndex = CurrentShownCordinate.OrderIndex,
            Ships = shipData
        };

        // Yükleniyor paneli kapatıyoruz.
        LoadingController.LC.ShowLoading();

        StartCoroutine(ApiService.API.Post("FlyNewFleet", requestData, (ApiResult response) =>
        {
            // Yükleniyor paneli kapatıyoruz.
            LoadingController.LC.CloseLoading();

            if (response.IsSuccess)
            {
                // Filoları yeniliyoruz.
                FleetController.FC.GetLatestFleets();

                // Gemileri envanterden siliyoruz.
                spyShip.ForEach(e => ShipyardController.SC.DestroyShip(e.Item1, e.Item2));

                // Toast mesajını basıyoruz ekrana.
                ToastController.TC.ShowToast(base.GetLanguageText("CasusGemisiGönderildi"));

                // Butonu yeniliyoruz.
                ValidateSpyShipCount();
            }
        }));
    }

    private void ValidateSpyShipCount()
    {
        // Gemi miktarını basıyoruz.
        int spyShipCount = LoginController.LC.CurrentUser.UserPlanetShips.Count(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == Ships.CasusSondası);

        // Gönderilebilir gemi miktarı
        TXT_FastSpyQuantity.text = spyShipCount.ToString();

        // Eğer yeterli miktar yok ise butonu devredışı bırakıyoruz.
        if (spyShipCount > 0)
            FastSpyButton.interactable = true;
        else
            FastSpyButton.interactable = false;
    }

}
