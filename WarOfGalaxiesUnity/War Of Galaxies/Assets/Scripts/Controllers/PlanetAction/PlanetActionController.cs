using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetActionController : BasePanelController
{
    public static PlanetActionController PAC { get; set; }

    private void Awake()
    {
        if (PAC == null)
            PAC = this;
        else
            Destroy(gameObject);
    }

    [Serializable]
    public class FleetTypeStruct
    {
        public FleetTypes FleetType;
        public Button FleetTypeButton;
    }

    [Header("Filo içi yapılabilecek hareketlerin listesi.")]
    public List<FleetTypeStruct> FleetTypeButtons;

    [Header("Gezegen ismi ve kordinatı basılacak.")]
    public TMP_Text TXT_PlanetNameAndCordinate;

    [Header("Gezegen ismi ve kordinatı basılacak.")]
    public TMP_Text TXT_PlanetNameAndCordinateSecond;

    [Header("Gezegene yapılacak olan aksiyon türü.")]
    public TMP_Text TXT_FleetAction;

    [Header("Gezegene yapılacak olan aksiyon türü.")]
    public TMP_Text TXT_FleetActionSecond;

    [Header("Kullanılabilir gemileri basmak için kullanacağız.")]
    public GameObject UseableShipPref;

    [Header("Kullanılmış olan gemileri burdan türeteceğiz.")]
    public GameObject UsedShipPref;

    [Header("Kullanılabilir gemiler buraya koyulacak.")]
    public ScrollRect UseableShipsContent;

    [Header("Kullanılabilir gemiler buraya koyulacak.")]
    public ScrollRect UsedShipContent;

    [Header("Gemi seçimini yaptıktan sonraki devam et butonu.")]
    public Button ContinueButton;

    [Header("Herhangi bir gemi seçilmediğinde açılacak olan mesaj.")]
    public GameObject NoShipSelected;

    [Header("Gezegende gönderilebilecek gemi kalmadığında yazacak olan text.")]
    public GameObject ShipNotFoundInPlanet;

    [Header("Stepper buraya atıyoruz")]
    public BaseStepController Stepper;

    [Header("Seçili olan filo hareketi")]
    public FleetTypes CurrentFleetType = FleetTypes.None;

    [Header("Uçuş süresini buraya yazacağız.")]
    public TMP_Text TXT_FlightTime;

    [Header("Hedefe varış süresi.")]
    public TMP_Text TXT_ArriveDate;

    [Header("Dönüş süresi.")]
    public TMP_Text TXT_ReturnDate;

    [Header("Gemilerin uçuş hızı.")]
    public Slider SLIDER_ShipSpeed;

    [Header("Gemilerin uçuş hızı oranı.")]
    public TMP_Text TXT_ShipSpeedRate;

    [Header("Gemilerin yakacağı yakıt.")]
    public TMP_Text TXT_Fuel;

    [Header("Taşınacak kaynaklar.")]
    public TMP_Text TXT_CargoCapacity;

    [Header("Taşınmak istenen metal değeri.")]
    public TMP_InputField INP_CarryMetal;

    [Header("Taşınmak istenen kristal değeri.")]
    public TMP_InputField INP_CarryCrystal;

    [Header("Taşınmak istenen kristal değeri.")]
    public TMP_InputField INP_CarryBoron;

    [Header("Filoyu gönder butonu.")]
    public Button SendFleetButton;

    /// <summary>
    /// Taşınacak olan kaynaklar.
    /// </summary>
    private ResourcesDTO carriedResources = new ResourcesDTO(0, 0, 0);

    /// <summary>
    /// Gönderilebilecek olan gemilerin yer aldığı liste.
    /// </summary>
    public List<ShipItemCanSendController> ShipsCanSend = new List<ShipItemCanSendController>();

    /// <summary>
    /// Gönderilecek olan gemilerin listesi.
    /// </summary>
    public List<ShipItemToSendController> ShipsToSend = new List<ShipItemToSendController>();

    /// <summary>
    /// Gösterilen kordinat.
    /// </summary>
    public CordinateDTO CurrentCordinate { get; set; }

    /// <summary>
    /// Gösterilen gezegen bilgisi.
    /// </summary>
    public SolarPlanetDTO CurrentSelectedPlanetInfo { get; set; }

    protected override void Start()
    {
        base.Start();

        // Her bir butona görev yüklüyoruz.
        FleetTypeButtons.ForEach(e => e.FleetTypeButton.onClick.AddListener(() => OnActionChanged(e.FleetType)));

        // Sayfa değiştiğinde çalışacak.
        Stepper.OnStepChanged += new EventHandler<BaseStepController.StepEventArgs>((s, e) =>
        {
            if (e.CurrentStep == 2)
            {
                // Gemiler ve miktarları.
                IEnumerable<Tuple<Ships, int>> shipsWithQuantity = ShipsToSend.Select(x => new Tuple<Ships, int>(x.UserPlanetShip.ShipId, x.Quantity));

                // Filo kapasitesini ekrana basıyoruz.
                double fleetCapacity = FleetExtends.CalculateShipCapacity(shipsWithQuantity);

                // Eğer depo daha az ise taşınan kaynakları sıfırlıyoruz.
                if (carriedResources.Sum() > fleetCapacity)
                {
                    carriedResources = ResourcesDTO.ResourceZero;
                    INP_CarryMetal.text = string.Empty;
                    INP_CarryCrystal.text = string.Empty;
                    INP_CarryBoron.text = string.Empty;
                    OnResourceEdit();
                }
            }
        });
    }

    public void Load(SolarPlanetDTO planetInfo, CordinateDTO cordinateInfo)
    {
        // Seçilen gezegenin bilgileri.
        CurrentSelectedPlanetInfo = planetInfo;

        // Kordinat bilgisini basıyoruz.
        CurrentCordinate = cordinateInfo;

        // Gezegen seçilmiş ise ismini basıyoruz.
        if (CurrentSelectedPlanetInfo != null && CurrentSelectedPlanetInfo.UserPlanet != null)
        {
            TXT_PlanetNameAndCordinate.text = $"<color=red>{LanguageController.LC.GetText("Hedef")} :</color> {planetInfo.UserPlanet.PlanetName} <color=orange>({CordinateExtends.ToCordinateString(cordinateInfo)})</color>";
            TXT_PlanetNameAndCordinateSecond.text = TXT_PlanetNameAndCordinate.text;
        }
        else
        {
            TXT_PlanetNameAndCordinate.text = $"<color=red>{LanguageController.LC.GetText("Hedef")} :</color> {LanguageController.LC.GetText("KeşfedilmemişGezegen")} <color=orange>({CordinateExtends.ToCordinateString(cordinateInfo)})</color>";
            TXT_PlanetNameAndCordinateSecond.text = TXT_PlanetNameAndCordinate.text;
        }

        // Eğer oyuncuya ait bir gezgen ise saldır ve casusluk yapı kapatacağız.
        UserPlanetCordinatesDTO userPlanet = LoginController.LC.CurrentUser.UserPlanetCordinates.Find(x => cordinateInfo.Equals(x.GalaxyIndex, x.SolarIndex, x.OrderIndex));

        if (userPlanet != null)
        {
            // Saldır, Casusluk, Sömürgeleştir butonlarını kapatıyoruz.
            FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Saldır).FleetTypeButton.gameObject.SetActive(false);
            FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Casusluk).FleetTypeButton.gameObject.SetActive(false);
            FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Sömürgeleştir).FleetTypeButton.gameObject.SetActive(false);

            // Eğer şu an seçili olan gezegene tıklanmış ise nakliye de kapatılacak. Her hangi bir aksiyon alınamayacak.
            if (GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId == userPlanet.UserPlanetId)
            {
                FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Nakliye).FleetTypeButton.gameObject.SetActive(false);
                FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Konuşlandır).FleetTypeButton.gameObject.SetActive(false);
            }
        }
        else // Demekki burada bir gezegen yok ozaman saldır ve casusluk da kapalı olacak.
        {
            if (planetInfo == null)
            {
                FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Saldır).FleetTypeButton.gameObject.SetActive(false);
                FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Casusluk).FleetTypeButton.gameObject.SetActive(false);
                FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Nakliye).FleetTypeButton.gameObject.SetActive(false);
                FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Konuşlandır).FleetTypeButton.gameObject.SetActive(false);
            }
            else
            {
                FleetTypeButtons.Find(x => x.FleetType == FleetTypes.Sömürgeleştir).FleetTypeButton.gameObject.SetActive(false);
            }
        }

        // Gezegendeki gemiler.
        IEnumerable<UserPlanetShipDTO> userPlanetShips = LoginController.LC.CurrentUser.UserPlanetShips.Where(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Eğer gemi var ise gemi bulunamadı panelini kapatıyoruz.
        if (userPlanetShips.Count() > 0)
            ShipNotFoundInPlanet.SetActive(false);

        // Gezegendeki gemileri alıyoruz.
        foreach (UserPlanetShipDTO userPlanetShip in userPlanetShips)
        {
            // Gemi resimini oluşturuyoruz.
            GameObject ship = Instantiate(UseableShipPref, UseableShipsContent.content);

            // Gemileri yüklüyoruz..
            ShipItemCanSendController sicsc = ship.GetComponent<ShipItemCanSendController>();

            // Listeye ekliyoruz.
            ShipsCanSend.Add(sicsc);

            // Bilgileri yüklüyoruz.
            sicsc.LoadData(userPlanetShip);
        }

        // Default bir action butonunu seçiyoruz.
        FleetTypeStruct defaultAction = FleetTypeButtons.Find(x => x.FleetTypeButton.gameObject.activeSelf);

        // Default actionı seçiyoruz.
        if (defaultAction != null)
            OnActionChanged(defaultAction.FleetType);

        // İkinci sayfaya geçtiğinde her saniye ekranda görünen dataları güncelleyeceğiz.
        StartCoroutine(OnStepSecond());
    }

    /// <summary>
    /// Filoyu yola çıkarır.
    /// </summary>
    public void SendFleet()
    {
        #region Casusluk Kontrolü

        // Casusluk hareketinde yalnızca casusluk gemisi gönderebilir.
        if (CurrentFleetType == FleetTypes.Casusluk)
        {
            // Başka bir gemi varsa filo da ozaman uyarı veriyoruz.
            bool isOtherShipExists = ShipsToSend.Any(x => x.UserPlanetShip.ShipId != Ships.CasusSondası);

            // Eğer başka gemi var ise hata dönüyoruz.
            if (isOtherShipExists)
            {
                // Uyarı çıkıyoruz.
                ToastController.TC.ShowToast(base.GetLanguageText("GeçersizGemiCasusluk"));

                // Geri dönüyoruz.
                return;
            }
        }

        #endregion

        string shipData = FleetExtends.ShipDataToStringData(ShipsToSend.Select(x => new Tuple<Ships, int>(x.UserPlanetShip.ShipId, x.Quantity)));

        // Uçuş bilgileri.
        SendFleetFromPlanetDTO requestData = new SendFleetFromPlanetDTO
        {
            SenderUserPlanetId = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId,
            CarriedBoron = carriedResources.Boron,
            CarriedCrystal = carriedResources.Crystal,
            CarriedMetal = carriedResources.Metal,
            FleetSpeed = SLIDER_ShipSpeed.value / 100,
            FleetType = (int)CurrentFleetType,
            DestinationGalaxyIndex = CurrentCordinate.GalaxyIndex,
            DestinationSolarIndex = CurrentCordinate.SolarIndex,
            DestinationOrderIndex = CurrentCordinate.OrderIndex,
            Ships = shipData
        };

        // Yükleniyor panelini açıyoruz.
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
                ShipsToSend.ForEach(e => ShipyardController.SC.DestroyShip(e.UserPlanetShip.ShipId, e.Quantity));

                // Taşınan kaynakları depodan çıkıyoruz.
                GlobalPlanetController.GPC.CurrentPlanet.ReducePlanetResources(carriedResources);

                // Toast mesajını basıyoruz ekrana.
                ToastController.TC.ShowToast(base.GetLanguageText("FilonYolaÇıktı"));

                // Footer paneli kapatıp açıyoruz.
                if (PlanetActionFooterPanel.PAFP != null)
                    PlanetActionFooterPanel.PAFP.ShowCordinate(PlanetActionFooterPanel.PAFP.CurrentShownPlanet, PlanetActionFooterPanel.PAFP.CurrentShownCordinate);

                // Paneli artık kapatabiliriz.
                base.ClosePanel();
            }

        }));
    }

    public void OnActionChanged(FleetTypes fleetType)
    {
        // Önceki butonu tekrar aktif hale getirmeliyiz.
        if (CurrentFleetType != FleetTypes.None)
            FleetTypeButtons.Find(x => x.FleetType == CurrentFleetType).FleetTypeButton.interactable = true;

        // Yenisini güncelliyoruz.
        CurrentFleetType = fleetType;

        // Butonu kapatıyoruz.
        FleetTypeButtons.Find(x => x.FleetType == CurrentFleetType).FleetTypeButton.interactable = false;

        // Seçili olan aksiyonu basıyoruz ekrana.
        TXT_FleetAction.text = LanguageController.LC.GetText($"FT{(int)fleetType}");

        // İkinci adımda da aksiyonu yazıyoruz bu yüzden basıyoruz.
        TXT_FleetActionSecond.text = TXT_FleetAction.text;
    }

    /// <summary>
    /// İkinci sayfadayken sayaçlar çalışacak.
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnStepSecond()
    {
        // YAlnızca ekran açık olduğunda çalışacak timer.
        yield return new WaitUntil(() => Stepper.CurrentStep == 2);

        // Gemi gidiş hızı.
        float shipSpeed = SLIDER_ShipSpeed.value / 100;

        // Uçuş süresi hesaplanacak.
        double minSpeed = FleetExtends.GetMinSpeedInFleet(ShipsToSend.Select(x => x.UserPlanetShip.ShipId));

        // Gezegenler arasındaki mesafe.
        double distance = FleetExtends.CalculateDistance(GlobalPlanetController.GPC.CurrentPlanetCordinate, CurrentCordinate);

        // Saniye cinsinden uçuş süresi.
        double flightTime = FleetExtends.CalculateFlightTime(distance, shipSpeed, minSpeed);

        // Ekrana uçuş süresini basıyoruz.
        TXT_FlightTime.text = $"<color=red>{base.GetLanguageText("UçuşSüresi")} ({base.GetLanguageText("TekYön")}) : </color> {TimeExtends.GetCountdownText(TimeSpan.FromSeconds(flightTime / 2))}";

        // Şuanı alıyoruz.
        DateTime currentDate = DateTime.Now;

        // Varış süresini hesaplayıp basıyoruz.
        TXT_ArriveDate.text = $"<color=red>{base.GetLanguageText("VarışTarihi")} : </color>{currentDate.AddSeconds(flightTime / 2).ToString("yyyy.MM.dd hh:mm:ss")}";

        // Varış süresini hesaplayıp basıyoruz.
        TXT_ReturnDate.text = $"<color=red>{base.GetLanguageText("DönüşTarihi")} : </color>{currentDate.AddSeconds(flightTime).ToString("yyyy.MM.dd hh:mm:ss")}";

        // Gemiler ve miktarları.
        IEnumerable<Tuple<Ships, int>> shipsWithQuantity = ShipsToSend.Select(x => new Tuple<Ships, int>(x.UserPlanetShip.ShipId, x.Quantity));

        // Yakıtı hesaplıyoruz.
        double fuelCost = FleetExtends.CalculateFuelCost(shipsWithQuantity, distance, shipSpeed);

        // Yakıt rengi beyaz.
        string fuelColor = "white";

        // Eğer uçuş maliyetini karşılamıyor ise rengi kırmızı yapıyoruz.
        if (fuelCost > GlobalPlanetController.GPC.CurrentPlanet.Boron)
        {
            // Yakıt rengi kırmızı yani yeterli değil.
            fuelColor = "red";

            // Butonu kapatıyoruz ki filoyu gönderemesin.
            SendFleetButton.interactable = false;

        }
        else // Uçuş maliyetini karşılıyor ise butonu açıyoruz.
            SendFleetButton.interactable = true;

        // Yakıtı ekrana basıyoruz.
        TXT_Fuel.text = $"<color=red>{base.GetLanguageText("YakıtTüketimi")} : </color><color={fuelColor}>{ResourceExtends.ConvertToDottedResource(fuelCost)} {base.GetLanguageText("Bor")}</color>";

        // Filo kapasitesini ekrana basıyoruz.
        double fleetCapacity = FleetExtends.CalculateShipCapacity(shipsWithQuantity);

        // Kapasite sınırına ulaşıldıysa renk kırmızı olacak.
        string capacityColor = "white";

        // Taşınan toplam.
        double carriedSum = carriedResources.Sum();

        // Eğer taşınan ve filo kapasitesi aynı ise kırmızı yakıyoruz.
        if (carriedSum == fleetCapacity)
            capacityColor = "red";

        // Yakıtı ekrana basıyoruz.
        TXT_CargoCapacity.text = $"{base.GetLanguageText("TaşınacakKaynaklar")}: <color={capacityColor}>({ResourceExtends.ConvertToDottedResource(carriedResources.Sum())} / {ResourceExtends.ConvertToDottedResource(fleetCapacity)})</color>";

        // 1 saniye sonra yeniliyoruz.
        yield return new WaitForSecondsRealtime(.5f);

        // Tekrar kendisini çağırıyoruz.
        StartCoroutine(OnStepSecond());
    }

    #region Slider ve Kaynak değerleri sıfırlandığında

    public void OnShipSpeedSliderValueChanged() => TXT_ShipSpeedRate.text = $"{(int)SLIDER_ShipSpeed.value}%";

    public void OnResourceEdit()
    {
        // Gemiler ve miktarları.
        IEnumerable<Tuple<Ships, int>> shipsWithQuantity = ShipsToSend.Select(x => new Tuple<Ships, int>(x.UserPlanetShip.ShipId, x.Quantity));

        // Filo kapasitesini ekrana basıyoruz.
        double fleetCapacity = FleetExtends.CalculateShipCapacity(shipsWithQuantity);

        #region Gönderilecek metali kontrol ediyoruz.

        double metal = 0;

        double.TryParse(INP_CarryMetal.text, out metal);

        if (metal != carriedResources.Metal)
        {
            if (metal + carriedResources.Crystal + carriedResources.Boron > fleetCapacity)
                metal = fleetCapacity - (carriedResources.Crystal + carriedResources.Boron);
        }

        #endregion

        #region Gönderilecek kristali kontrol ediyoruz.

        double crystal = 0;

        double.TryParse(INP_CarryCrystal.text, out crystal);

        if (crystal != carriedResources.Crystal)
        {
            if (crystal + carriedResources.Metal + carriedResources.Boron > fleetCapacity)
                crystal = fleetCapacity - (carriedResources.Metal + carriedResources.Boron);
        }

        #endregion

        #region Gönderilecek boronu kontrol ediyoruz.

        double boron = 0;

        double.TryParse(INP_CarryBoron.text, out boron);

        if (boron != carriedResources.Boron)
        {
            if (boron + carriedResources.Metal + carriedResources.Crystal > fleetCapacity)
                boron = fleetCapacity - (carriedResources.Metal + carriedResources.Crystal);
        }

        #endregion

        #region Dönüştürülmüş formatta ekrana basıyoruz.

        int metalTextLength = INP_CarryMetal.text.Length;
        int crystalTextLength = INP_CarryCrystal.text.Length;
        int boronTextLength = INP_CarryBoron.text.Length;

        #region Formata çeviriyoruz.

        carriedResources.Metal = metal;
        INP_CarryMetal.text = ResourceExtends.ConvertToDottedResource(metal);

        carriedResources.Crystal = crystal;
        INP_CarryCrystal.text = ResourceExtends.ConvertToDottedResource(crystal);

        carriedResources.Boron = boron;
        INP_CarryBoron.text = ResourceExtends.ConvertToDottedResource(boron);

        #endregion

        #region İmleci kaydırıyoruz.


        INP_CarryMetal.caretPosition += INP_CarryMetal.text.Length > metalTextLength ? INP_CarryMetal.text.Length - metalTextLength : metalTextLength - INP_CarryMetal.text.Length;
        INP_CarryCrystal.caretPosition += INP_CarryCrystal.text.Length > crystalTextLength ? INP_CarryCrystal.text.Length - crystalTextLength : crystalTextLength - INP_CarryCrystal.text.Length;
        INP_CarryBoron.caretPosition += INP_CarryBoron.text.Length > boronTextLength ? INP_CarryBoron.text.Length - boronTextLength : boronTextLength - INP_CarryBoron.text.Length;

        #endregion

        #endregion
    }

    #endregion

    #region Bütün kaynakları yükleme kısayolları

    public void LoadAllMetals()
    {
        INP_CarryMetal.text = ResourceExtends.ConvertToDottedResource(GlobalPlanetController.GPC.CurrentPlanet.Metal);
        OnResourceEdit();
    }

    public void LoadAllCrystals()
    {
        INP_CarryCrystal.text = ResourceExtends.ConvertToDottedResource(GlobalPlanetController.GPC.CurrentPlanet.Crystal);
        OnResourceEdit();
    }

    public void LoadAllBorons()
    {
        INP_CarryBoron.text = ResourceExtends.ConvertToDottedResource(GlobalPlanetController.GPC.CurrentPlanet.Boron);
        OnResourceEdit();
    }

    #endregion

    #region Kaynakları Sıfırlama.

    public void ResetMetals()
    {
        INP_CarryMetal.text = string.Empty;
        OnResourceEdit();
    }

    public void ResetCrystals()
    {
        INP_CarryCrystal.text = string.Empty;
        OnResourceEdit();
    }

    public void ResetBorons()
    {
        INP_CarryBoron.text = string.Empty;
        OnResourceEdit();
    }

    #endregion

}
