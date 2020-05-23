using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FleetPanelItemController : BaseLanguageBehaviour
{
    [Header("Gönderen gezegene ait ikonu basacağız.")]
    public Image IMG_SenderPlanetImage;

    [Header("Gönderen gezegenin ismini basacağız.")]
    public TMP_Text TXT_SenderPlanetName;

    [Header("Gönderen gezegenin kordinatını basacağız.")]
    public TMP_Text TXT_SenderPlanetCordinate;

    [Header("Gezegene geri dönüş tarihini basacağız.")]
    public TMP_Text TXT_SenderArriveDate;

    [Header("Hedef konuma ait resmi basacağız.")]
    public Image IMG_DestinationImage;

    [Header("Hedef konumun ismini basacağız.")]
    public TMP_Text TXT_DestinationName;

    [Header("Hedef konumun kordinatı basacağız.")]
    public TMP_Text TXT_DestinationCordinate;

    [Header("Hedefe ulaşmış olacağı tarihi basacağız.")]
    public TMP_Text TXT_DestinationArriveDate;

    [Header("Kalan süreyi ekrana basıyoruz.")]
    public TMP_Text TXT_LeftTime;

    [Header("Uçuşu göstermek için kullanılacak olan slider.")]
    public Slider SLIDER_FlightProgress;

    [Header("Yapılan aksiyon.")]
    public TMP_Text TXT_ActionType;

    [Header("Filoyu geri çağırmak için")]
    public Button BallBackButton;

    /// <summary>
    /// Gösterdiği filo bilgisi.
    /// </summary>
    public FleetDTO FleetInfo { get; set; }

    /// <summary>
    /// Hedefe varış süresi.
    /// </summary>
    public DateTime GetHalfOfFlyDate
    {
        get
        {
            // Uçuşa başlanılan tarih.
            DateTime fleetBeginFlyDate = FleetInfo.FleetLoadDate.AddSeconds(-FleetInfo.BeginPassedTime);

            // Filo hareketinin tamamlanmasına kalan süre.
            DateTime fleetEndFlyDate = FleetInfo.FleetLoadDate.AddSeconds(FleetInfo.EndLeftTime);

            // Toplam yolculuk süresi saniye cinsinden.
            double totalPathSeconds = (fleetEndFlyDate - fleetBeginFlyDate).TotalSeconds;

            // Toplam yolculuk süresi.
            double haflOfPath = totalPathSeconds / 2;

            return fleetBeginFlyDate.AddSeconds(haflOfPath);
        }
    }

    /// <summary>
    /// Gezegene dönüş süresi.
    /// </summary>
    public DateTime GetEndFlyDate
    {
        get
        {
            return FleetInfo.FleetLoadDate.AddSeconds(FleetInfo.EndLeftTime);
        }
    }

    public DateTime GetBeginFlyDate
    {
        get
        {
            return FleetInfo.FleetLoadDate.AddSeconds(-FleetInfo.BeginPassedTime);
        }
    }

    public IEnumerator LoadData(FleetDTO fleetInfo)
    {
        // Şuanki tarih.
        DateTime currentDate = DateTime.Now;

        // Uçuşa başlanılan tarih.
        DateTime fleetBeginFlyDate = GetBeginFlyDate;

        // Filo hareketinin tamamlanmasına kalan süre.
        DateTime fleetEndFlyDate = GetEndFlyDate;

        // Toplam yolculuk süresi saniye cinsinden.
        double totalPathSeconds = (fleetEndFlyDate - fleetBeginFlyDate).TotalSeconds;

        // Toplam yolculuk süresi.
        double haflOfPath = totalPathSeconds / 2;

        // Yolun yarısı.
        DateTime halfOfFlyDate = GetHalfOfFlyDate;

        // Filo bilgisini tutuyoruz.
        FleetInfo = fleetInfo;

        // Filo hareketini basıyoruz.
        TXT_ActionType.text = $"{base.GetLanguageText($"FT{(int)fleetInfo.FleetActionTypeId}")}";

        // Eğer dönüyor ise dönü yazıyoruz.
        if (currentDate > halfOfFlyDate)
            TXT_ActionType.text += $" (<color=green>{base.GetLanguageText("Dönüş")}</color>)";

        // Gönderenin adını basıyoruz.
        TXT_SenderPlanetName.text = fleetInfo.SenderPlanetName;

        // Gönderenin kordinatı.
        TXT_SenderPlanetCordinate.text = fleetInfo.SenderCordinate;

        // Gönderene geri dönüş tarihi
        TXT_SenderArriveDate.text = fleetEndFlyDate >= currentDate ? fleetEndFlyDate.ToString("yyyy.MM.dd hh:mm:ss") : "-";

        // Hedef konumun ismi.
        TXT_DestinationName.text = fleetInfo.DestinationPlanetName;

        // Hedef konum.
        TXT_DestinationCordinate.text = fleetInfo.DestinationCordinate;

        // Hedef konuma ulaşma tarihi.
        TXT_DestinationArriveDate.text = currentDate <= halfOfFlyDate ? halfOfFlyDate.ToString("yyyy.MM.dd hh:mm:ss") : "-";

        // Eğer yolun yarısında ise ekrana kalan süreyi basacağız. Ulaşmak için.
        if (currentDate <= halfOfFlyDate)
            TXT_LeftTime.text = TimeExtends.GetCountdownText(halfOfFlyDate - currentDate);
        else if (currentDate <= fleetEndFlyDate)
            TXT_LeftTime.text = TimeExtends.GetCountdownText(fleetEndFlyDate - currentDate);
        else
            TXT_LeftTime.text = base.GetLanguageText("Tamamlandı");

        // Yuvarlak miktarını alıyoruz. Buna göre hesaplama yapacağız.
        float countOfCircle = SLIDER_FlightProgress.maxValue;

        int quantityOfCircleInProg;

        // Gidiş geliş olmasına bağlı olarak slider efekti çalışacak.
        if (currentDate < halfOfFlyDate)
        {
            // Kalan süre.
            double leftTimeToArrive = (halfOfFlyDate - currentDate).TotalSeconds;

            // Bu kadar saniyede bir yuvarlak tamamlanacak.
            double each = haflOfPath / countOfCircle;

            // Tamamlanmış olan yuvarlak miktarı.
            quantityOfCircleInProg = (int)countOfCircle - Mathf.FloorToInt((float)(leftTimeToArrive / each));
        }
        else
        {
            // Kalan süre.
            double leftTimeToReturn = (fleetEndFlyDate - currentDate).TotalSeconds;

            // Bu kadar saniyede bir yuvarlak tamamlanacak.
            double each = haflOfPath / countOfCircle;

            // Tamamlanmış olan yuvarlak miktarı.
            quantityOfCircleInProg = Mathf.FloorToInt((float)(leftTimeToReturn / each));

            // Eğer buton açık ise kapatıyoruz.
            if (BallBackButton.gameObject.activeSelf)
                BallBackButton.gameObject.SetActive(false);
        }

        // Slideri ayarlıyoruz.
        SLIDER_FlightProgress.value = quantityOfCircleInProg;

        // Ufak bir animasyon için resmi buluyoruz slider butonuna ait.
        Image sliderHandlerImage = SLIDER_FlightProgress.handleRect.GetComponent<Image>();

        // Tersine çevirip on of effekti yapııyoruz.
        sliderHandlerImage.enabled = !sliderHandlerImage.enabled;

        // 1 saniye sonra kendisini çağırıp yeniliyoruz.
        yield return new WaitForSecondsRealtime(1f);

        // Kendisini tekrar çağırıyoruz.
        StartCoroutine(LoadData(fleetInfo));
    }

    public void OnClickCallBackFleet()
    {

    }

}
