using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipyardItemController : MonoBehaviour
{
    [Header("Aktif tuttuğu gemi.")]
    public Ships CurrentShip;

    [Header("Geminin ismini basıyoruz.")]
    public TextMeshProUGUI ShipName;

    [Header("Kullanıcının sahip olduğu miktar.")]
    public TextMeshProUGUI ShipCount;

    [Header("Geminin resmi.")]
    public Image ShipImage;

    [Header("Geri sayım resmi.")]
    public Image CountdownImage;

    [Header("Geri sayım süresi.")]
    public TextMeshProUGUI CountdownText;

    /// <summary>
    /// Aktif gemi üretimi
    /// </summary>
    public UserPlanetShipProgDTO CurrentProg { get; set; }

    public IEnumerator LoadShipDetails(Ships ship)
    {
        // Gemi bilgisi.
        CurrentShip = ship;

        // Aktif gemi miktarı.
        UserPlanetShipDTO currentShipCount = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == ship);

        // Resmi yüklüyoruz.
        ShipImage.sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == ship).ShipImage;

        // Üretim var mı?
        UserPlanetShipProgDTO prog = LoginController.LC.CurrentUser.UserPlanetShipProgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == ship);

        // Eğer üretim var ise süreyi basıyoruz.
        if (prog != null)
        {
            // Eğer üretim var ise basıyoruz.
            CurrentProg = prog;

            // İkonu ve kalan süreyi basıyoruz.
            CountdownImage.gameObject.SetActive(true);

            // Tersanesini buluyoruz.
            UserPlanetBuildingDTO shipyard = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

            // Bir geminin üretimi için gereken süre.
            double countdownOneItem = StaticData.CalculateShipCountdown(ship, shipyard == null ? 0 : shipyard.BuildingLevel);

            // Birim başına baktıktan sonra tamamlanmasına kalan süreye bakıyoruz.
            DateTime completeTime = prog.LastVerifyDate.AddSeconds(-prog.OffsetTime).AddSeconds(countdownOneItem);

            // Tamamlanmasına kalan süre.
            TimeSpan leftTime = completeTime - DateTime.UtcNow;

            // Üretim geri sayımını aktif ediyoruz.
            CountdownText.text = $"({prog.ShipCount}){Environment.NewLine}{TimeExtends.GetCountdownText(leftTime)}";

            // Eğer üretim süresi bittiyse.
            if (leftTime.TotalSeconds <= 0)
            {
                // Ve yeni üretimlere başlıyoruz.
                prog.LastVerifyDate = DateTime.UtcNow;

                // Yarım üretimi 0lıyoruz.
                prog.OffsetTime = 0;

                // Üretilecek gemi miktarını 1 azaltıyoruz
                prog.ShipCount--;

                // Daha önce bu gemiye sahip miydi?
                if (currentShipCount == null)
                {
                    // Eğer ilk defa ekleniyor ise yeni oluşturuyoruz.
                    currentShipCount = new UserPlanetShipDTO() { ShipCount = 1, ShipId = prog.ShipId, UserPlanetId = prog.UserPlanetId };

                    // Listeye ekliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetShips.Add(currentShipCount);
                }
                else // AKsi durumda sadece miktarı arttırıyoruz.
                    currentShipCount.ShipCount++;

                // Eğer gemi kalmamış ise siliyoruz.
                if (prog.ShipCount <= 0)
                {
                    // Eğer daha yok ise listeden siliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetShipProgs.Remove(prog);

                    // Sonrakinin başlangıç tarihini güncelliyoruz.
                    UserPlanetShipProgDTO nextProg = LoginController.LC.CurrentUser.UserPlanetShipProgs.FirstOrDefault();

                    // Sonraki üretim.
                    if (nextProg != null)
                        nextProg.LastVerifyDate = DateTime.UtcNow;
                }

                // Kuyruğu yeniliyoruz.
                ShipyardQueueController.SQC.RefreshShipyardQueue();
            }

            // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
            ShipCount.text = currentShipCount == null ? $"{0}" : $"{currentShipCount.ShipCount}".ToString();
        }
        else
        {
            // İkonu kapatıyoruz.
            CountdownImage.gameObject.SetActive(false);

            // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
            ShipCount.text = currentShipCount == null ? $"{0}" : $"{currentShipCount.ShipCount}";
        }

        // 1 saniye bekleyip tekrar çağırıyoruz.
        yield return new WaitForSecondsRealtime(1);

        // Tekrar çağırıyoruz.
        StartCoroutine(LoadShipDetails(ship));
    }

    public void ShowShipDetail()
    {
        GameObject shipyardDetailPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.ShipyardDetailPanel);
        ShipyardDetailItemPanel sdip = shipyardDetailPanel.GetComponent<ShipyardDetailItemPanel>();
        sdip.StartCoroutine(sdip.LoadShipDetals(CurrentShip));
    }

}
