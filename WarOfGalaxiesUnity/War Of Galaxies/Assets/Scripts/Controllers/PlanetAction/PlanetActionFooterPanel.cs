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
    public static PlanetActionFooterPanel PAFP { get; set; }

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

    [Header("Giden gemi sayısını buraya koyuyoruz.")]
    public TMP_Text TXT_GarbageAlreadySendMessage;

    [Header("Hızlı gönderme butonu.")]
    public Button BTN_FastSendGarbage;

    /// <summary>
    /// Gösterilen kordinata ait bilgiler.
    /// </summary>
    public CordinateDTO CurrentShownCordinate { get; set; }

    /// <summary>
    /// Gösterilen gezegen bilgisi.
    /// </summary>
    public SolarPlanetDTO CurrentShownPlanet { get; set; }

    private void Awake()
    {
        if (PAFP == null)
            PAFP = this;
        else
            Destroy(gameObject);
    }

    protected override void Update()
    {
        base.Update();

        // Boş alana tıklandığında paneli kapatıyoruz.
        if (Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null && !GlobalPanelController.GPC.IsAnyPanelOpen)
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
        // Eğer keşfedilmemiş gezegen ise kaynak yoktur.
        if (this.CurrentShownPlanet == null)
        {
            // Paneli kapatıyoruz.
            if (GarbagePanel.activeSelf)
                GarbagePanel.SetActive(false);

            // Devamına inmeye gerek yok.
            return;
        }

        // Eğer enkaz var ise gösteriyoruz.
        if (this.CurrentShownPlanet.GarbageMetal == 0 && this.CurrentShownPlanet.GarbageCrystal == 0 && this.CurrentShownPlanet.GarbageBoron == 0)
        {
            // Paneli kapatıyoruz.
            if (GarbagePanel.activeSelf)
                GarbagePanel.SetActive(false);

            // Devamına inmeye gerek yok.
            return;
        }

        // Çöp panelini açıyoruz.
        GarbagePanel.SetActive(true);

        // Toplanabilir metal miktarını basıyoruz.
        TXT_GarbageMetalQuantity.text = ResourceExtends.ConvertToDottedResource(this.CurrentShownPlanet.GarbageMetal);

        // Toplanabilir kristal miktarını basıyoruz.
        TXT_GarbageCrystalQuantity.text = ResourceExtends.ConvertToDottedResource(this.CurrentShownPlanet.GarbageCrystal);

        // Toplanabilir bor miktarını basıyoruz.
        TXT_GarbageBoronQuantity.text = ResourceExtends.ConvertToDottedResource(this.CurrentShownPlanet.GarbageBoron);

        // Çöp gemisi bilgisi.
        ShipDataDTO shipInfo = DataController.DC.GetShip(Ships.EnkazToplamaGemisi);

        // Toplam gidiyor olan gemi miktarı
        int goingShipQuantity = FleetController.FC.Fleets.Where(x => !x.IsReturnFleet && x.FleetActionTypeId == FleetTypes.Sök && x.DestinationCordinate == CordinateExtends.ToCordinateString(CurrentShownCordinate)).Select(x =>
        {
            List<Tuple<Ships, int>> fleet = FleetExtends.FleetDataToShipData(x.FleetData);
            Tuple<Ships, int> garbageCollector = fleet.Find(y => y.Item1 == Ships.EnkazToplamaGemisi);
            if (garbageCollector != null)
                return garbageCollector.Item2;
            else
                return 0;
        }).DefaultIfEmpty(0).Sum();

        // Gereken çöp aracı miktarı.
        int requiredGarbageShipQuantity = Mathf.CeilToInt((float)(CurrentShownPlanet.GarbageMetal + CurrentShownPlanet.GarbageCrystal + CurrentShownPlanet.GarbageBoron) / shipInfo.CargoCapacity);

        // Gezegendeki çöp toplayıcıları buluyoruz.
        UserPlanetShipDTO userPlanetGarbage = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.ShipId == Ships.EnkazToplamaGemisi && x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Çöp toplama aracı sayısı.
        int garbageCollectorCount = 0;

        // Eğer gezegende çöp toplayıcı var ise miktarı yazıyoruzç
        if (userPlanetGarbage != null)
            garbageCollectorCount = userPlanetGarbage.ShipCount;

        // Eğer hiç geri dönüşümcü yok ise uyarı ver.
        if (goingShipQuantity == 0 && garbageCollectorCount == 0)
        {
            // Eğer gönderebileceği gemi yok ise butonu kapatıyoruz.
            BTN_FastSendGarbage.interactable = false;

            // Texte de gereken gemi miktarını basıyoruz.
            TXT_GarbageAlreadySendMessage.text = base.GetLanguageText("GeriDönüşümcüGerekli", requiredGarbageShipQuantity.ToString());

            // Texti de kırmızıya boyuyoruz.
            TXT_GarbageAlreadySendMessage.color = Color.red;
        }
        else
        {
            // Eğer giden gemi var ise metni açıyoruz.
            if (goingShipQuantity > 0)
            {
                // Gönderilen gemi miktarını basıyoruz.
                TXT_GarbageAlreadySendMessage.text = base.GetLanguageText("GemiGidiyor", goingShipQuantity.ToString(), requiredGarbageShipQuantity.ToString());

                // Eğer maksimum gemi gönderiliyor ise butonu kapatıyoruz.
                if (goingShipQuantity == requiredGarbageShipQuantity)
                {
                    // Butonu da kapatıyoruz.
                    BTN_FastSendGarbage.interactable = false;

                    // Yazı rengi de kırmızı olacak belirgin.
                    TXT_GarbageAlreadySendMessage.color = Color.red;
                }
                else
                {
                    // Butonu da açıyoruz.
                    BTN_FastSendGarbage.interactable = true;

                    // Eğer hala gemi gönderebilir isek yeşil yapıyoruz butonu.
                    TXT_GarbageAlreadySendMessage.color = Color.green;
                }
            }
            else // Hiç gemisi gitmiyor demekki.
            {
                // Gemisi olmadığını söylüyoruz.
                TXT_GarbageAlreadySendMessage.text = base.GetLanguageText("GeriDönüşümcüGerekli", requiredGarbageShipQuantity.ToString());

                // Butonu da açıyoruz.
                BTN_FastSendGarbage.interactable = true;

                // Yazı rengi de kırmızı olacak belirgin. Turuncu.
                TXT_GarbageAlreadySendMessage.color = new Color32(255, 134, 0, 255);
            }
        }
    }

    public void ClickAllAction()
    {
        // Paneli açıyoruz.
        GameObject gpap = GlobalPanelController.GPC.ShowPanel(PanelTypes.GalaxyPlanetActionPanel);

        // Panele dataları yüklüyoruz.
        gpap.GetComponent<PlanetActionController>().Load(this.CurrentShownPlanet, this.CurrentShownCordinate);
    }

    public void FastSpy()
    {
        int spyCount = int.Parse(TXT_FastSpyQuantity.text);

        // Gönderilecek olan gemiler. Sadece casusluk.
        List<Tuple<Ships, int>> spyShip = new List<Tuple<Ships, int>> { new Tuple<Ships, int>(Ships.CasusDronu, spyCount) };

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

        ApiService.API.Post("FlyNewFleet", requestData, (ApiResult response) =>
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
        });
    }

    public void FastHarvestGarbage()
    {
        // Çöp gemisi bilgisi.
        ShipDataDTO shipInfo = DataController.DC.GetShip(Ships.EnkazToplamaGemisi);

        // Gereken çöp aracı miktarı.
        int requiredGarbageShipQuantity = Mathf.CeilToInt((float)(CurrentShownPlanet.GarbageMetal + CurrentShownPlanet.GarbageCrystal + CurrentShownPlanet.GarbageBoron) / shipInfo.CargoCapacity);

        // Gezegendeki çöp toplayıcıları buluyoruz.
        UserPlanetShipDTO userPlanetGarbage = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.ShipId == Ships.EnkazToplamaGemisi && x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Toplam gidiyor olan gemi miktarı
        int goingShipQuantity = FleetController.FC.Fleets.Where(x => x.DestinationCordinate == CordinateExtends.ToCordinateString(CurrentShownCordinate)).Select(x =>
        {
            List<Tuple<Ships, int>> fleet = FleetExtends.FleetDataToShipData(x.FleetData);
            Tuple<Ships, int> garbageCollector = fleet.Find(y => y.Item1 == Ships.EnkazToplamaGemisi);
            if (garbageCollector != null)
                return garbageCollector.Item2;
            else
                return 0;
        }).DefaultIfEmpty(0).Sum();

        // Hızlı şekilde gönderilecek olan gemi miktarı.
        int sendShipQuantity = requiredGarbageShipQuantity - goingShipQuantity;

        // Eğer miktar 0 dan küçük yada eşit ise geri dön.
        if (sendShipQuantity <= 0)
            return;

        // Kullanıcının sahip olduğu geri dönüşümcü miktarı.
        int userShipQuantity = LoginController.LC.CurrentUser.UserPlanetShips
            .Where(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == Ships.EnkazToplamaGemisi)
            .Select(x => x.ShipCount)
            .DefaultIfEmpty(0)
            .Sum();

        // Eğer sahip olduğumuz gemiden fazla göndermeye çalışıyor isek göndermeyeceğiz.
        if (sendShipQuantity > userShipQuantity)
            sendShipQuantity = userShipQuantity;

        // Gönderilecek olan geri dönüşümcüler.
        List<Tuple<Ships, int>> shipsToSend = new List<Tuple<Ships, int>> { new Tuple<Ships, int>(Ships.EnkazToplamaGemisi, sendShipQuantity) };

        // Datayı alıyoruz.
        string shipData = FleetExtends.ShipDataToStringData(shipsToSend);

        // Uçuş bilgileri.
        SendFleetFromPlanetDTO requestData = new SendFleetFromPlanetDTO
        {
            SenderUserPlanetId = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId,
            CarriedBoron = 0,
            CarriedCrystal = 0,
            CarriedMetal = 0,
            FleetSpeed = 1,
            FleetType = (int)FleetTypes.Sök,
            DestinationGalaxyIndex = CurrentShownCordinate.GalaxyIndex,
            DestinationSolarIndex = CurrentShownCordinate.SolarIndex,
            DestinationOrderIndex = CurrentShownCordinate.OrderIndex,
            Ships = shipData
        };

        // Yükleniyor panelini açıyoruz.
        LoadingController.LC.ShowLoading();

        ApiService.API.Post("FlyNewFleet", requestData, (ApiResult response) =>
        {
            // Yükleniyor paneli kapatıyoruz.
            LoadingController.LC.CloseLoading();

            if (response.IsSuccess)
            {
                // Filoları yeniliyoruz.
                FleetController.FC.GetLatestFleets();

                // Gemileri envanterden siliyoruz.
                shipsToSend.ForEach(e => ShipyardController.SC.DestroyShip(e.Item1, e.Item2));

                // Toast mesajını basıyoruz ekrana.
                ToastController.TC.ShowToast(base.GetLanguageText("FilonYolaÇıktı"));

                // Footer paneli kapatıp açıyoruz.
                base.ClosePanel();
            }

        });
    }

    private void ValidateSpyShipCount()
    {
        // Eğer kullanıcının gezegeni ise ozaman spy butonu kalkacak.
        if (LoginController.LC.CurrentUser.UserPlanetCordinates.Any(x => x.Same(CurrentShownCordinate)))
        {
            // Butonu kaldırıyoruz.
            FastSpyButton.gameObject.SetActive(false);

            // Geri dönüyoruz.
            return;
        }

        // Eğer burası keşfedilmemiş gezegen ise ozaman da casusluk kapalı olacak.
        if (this.CurrentShownPlanet == null)
        {
            // Butonu kaldırıyoruz.
            FastSpyButton.gameObject.SetActive(false);

            // Geri dönüyoruz.
            return;
        }

        // Butonu açıyoruz. Demekki casusluk yapılabilir bir gezegen.
        FastSpyButton.gameObject.SetActive(true);

        // Gemi miktarını basıyoruz.
        int spyShipCount = LoginController.LC.CurrentUser.UserPlanetShips.Count(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == Ships.CasusDronu);

        // Gönderilebilir gemi miktarı
        TXT_FastSpyQuantity.text = spyShipCount.ToString();

        // Eğer yeterli miktar yok ise butonu devredışı bırakıyoruz.
        if (spyShipCount > 0)
            FastSpyButton.interactable = true;
        else
            FastSpyButton.interactable = false;
    }

}
