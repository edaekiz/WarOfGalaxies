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
    public Button CallbackButton;

    /// <summary>
    /// Gösterdiği filo bilgisi.
    /// </summary>
    public FleetDTO FleetInfo { get; set; }

    public IEnumerator LoadData(FleetDTO fleetInfo)
    {
        // Şuanki tarih.
        DateTime currentDate = DateTime.Now;

        // Uçuşa başlanılan tarih.
        DateTime fleetBeginFlyDate = fleetInfo.FleetLoadDate.AddSeconds(-FleetInfo.BeginPassedTime);

        // Filo hareketinin tamamlanmasına kalan süre.
        DateTime fleetEndFlyDate = fleetInfo.FleetLoadDate.AddSeconds(FleetInfo.EndLeftTime);

        // Toplam uçuş süresi.
        double flightTime = (fleetEndFlyDate - fleetBeginFlyDate).TotalSeconds;

        // Filo bilgisini tutuyoruz.
        FleetInfo = fleetInfo;

        // Filo hareketini basıyoruz.
        TXT_ActionType.text = $"{base.GetLanguageText($"FT{(int)fleetInfo.FleetActionTypeId}")}";

        // Eğer dönüyor ise dönü yazıyoruz.
        if (fleetInfo.IsReturnFleet)
            TXT_ActionType.text += $" (<color=green>{base.GetLanguageText("Dönüş")}</color>)";

        // Gönderenin adını basıyoruz.
        TXT_SenderPlanetName.text = fleetInfo.SenderPlanetName;

        // Gönderenin kordinatı.
        TXT_SenderPlanetCordinate.text = fleetInfo.SenderCordinate;

        // Gönderene geri dönüş tarihi
        TXT_SenderArriveDate.text = fleetEndFlyDate.AddSeconds(flightTime).ToString("yyyy.MM.dd hh:mm:ss");

        // Hedef konumun ismi.
        TXT_DestinationName.text = fleetInfo.DestinationPlanetName;

        // Hedef konum.
        TXT_DestinationCordinate.text = fleetInfo.DestinationCordinate;

        // Hedef konuma ulaşma tarihi.
        TXT_DestinationArriveDate.text = fleetEndFlyDate.ToString("yyyy.MM.dd hh:mm:ss");

        // Eğer yolun yarısında ise ekrana kalan süreyi basacağız. Ulaşmak için.
        if (currentDate <= fleetEndFlyDate)
            TXT_LeftTime.text = TimeExtends.GetCountdownText(fleetEndFlyDate - currentDate);
        else
            TXT_LeftTime.text = base.GetLanguageText("Tamamlandı");

        // Yuvarlak miktarını alıyoruz. Buna göre hesaplama yapacağız.
        float countOfCircle = SLIDER_FlightProgress.maxValue;

        int quantityOfCircleInProg = 0;

        // Gidiş geliş olmasına bağlı olarak slider efekti çalışacak.
        if (!fleetInfo.IsReturnFleet && currentDate < fleetEndFlyDate)
        {
            // Kalan süre.
            double leftTimeToArrive = (fleetEndFlyDate - currentDate).TotalSeconds;

            // Bu kadar saniyede bir yuvarlak tamamlanacak.
            double each = flightTime / countOfCircle;

            // Tamamlanmış olan yuvarlak miktarı.
            quantityOfCircleInProg = (int)countOfCircle - Mathf.FloorToInt((float)(leftTimeToArrive / each));
        }
        else if (fleetInfo.IsReturnFleet)
        {
            // Kalan süre.
            double leftTimeToReturn = (fleetEndFlyDate - currentDate).TotalSeconds;

            // Bu kadar saniyede bir yuvarlak tamamlanacak.
            double each = flightTime / countOfCircle;

            // Tamamlanmış olan yuvarlak miktarı.
            quantityOfCircleInProg = Mathf.FloorToInt((float)(leftTimeToReturn / each));

            // Eğer buton açık ise kapatıyoruz.
            if (CallbackButton.gameObject.activeSelf)
                CallbackButton.gameObject.SetActive(false);
        }
        else
        {
            quantityOfCircleInProg = (int)countOfCircle;
        }

        // Slideri ayarlıyoruz.
        SLIDER_FlightProgress.value = quantityOfCircleInProg;

        // Ufak bir animasyon için resmi buluyoruz slider butonuna ait.
        Image sliderHandlerImage = SLIDER_FlightProgress.handleRect.GetComponent<Image>();

        // Tersine çevirip on of effekti yapııyoruz.
        sliderHandlerImage.enabled = !sliderHandlerImage.enabled;

        // Eğer gönderen kullanıcı ise ve filo geri dönmüyor ise butonu gösterebiliriz.
        if (LoginController.LC.CurrentUser.UserData.UserId == this.FleetInfo.SenderUserId && !this.FleetInfo.IsReturnFleet)
        {
            if (!CallbackButton.gameObject.activeSelf)
                CallbackButton.gameObject.SetActive(true);
        }else // Eğer geri dönüş filosu ya da kendisine gelen bir düşman ise geri dön butonu kapalı olacak.
        {
            if (CallbackButton.gameObject.activeSelf)
                CallbackButton.gameObject.SetActive(false);
        }

        // 1 saniye sonra kendisini çağırıp yeniliyoruz.
        yield return new WaitForSecondsRealtime(1f);

        // Kendisini tekrar çağırıyoruz.
        StartCoroutine(LoadData(fleetInfo));
    }

    public void OnClickCallBackFleet()
    {
        // Butonu kapatıyoruz.
        CallbackButton.interactable = false;

        // Geri çağırıyoruz.
        StartCoroutine(ApiService.API.Post("CallBackFlyFleet", new CallbackFleetDTO { FleetId = this.FleetInfo.FleetId }, (ApiResult response) =>
        {
            // Butonu tekrar açıyoruz.
            CallbackButton.interactable = true;

            // Yanıt başarılı ise.
            if (response.IsSuccess)
            {
                // Geri döndüğüne göre gidiş filosunu silebiliriz.
                FleetController.FC.Fleets.Remove(this.FleetInfo);

                // Datalarını geri getiriyoruz.
                FleetController.FC.RefreshReturnFleetData(this.FleetInfo.ReturnFleetId);
            }
        }));
    }

}
