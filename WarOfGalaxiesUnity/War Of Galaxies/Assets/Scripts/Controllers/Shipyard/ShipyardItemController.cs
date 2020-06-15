using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipyardItemController : BaseLanguageBehaviour
{
    [Header("Aktif tuttuğu gemi.")]
    public Ships CurrentShip;

    [Header("Geminin ismini basıyoruz.")]
    public TMP_Text ShipName;

    [Header("Kullanıcının sahip olduğu miktar.")]
    public TMP_Text ShipCount;

    [Header("Geminin resmi.")]
    public Image ShipImage;

    [Header("Geri sayım resmi.")]
    public Image CountdownImage;

    [Header("Geri sayım süresi.")]
    public TMP_Text CountdownText;

    public IEnumerator LoadShipDetails(Ships ship)
    {
        // Gemi bilgisi.
        CurrentShip = ship;

        // Gemi ismi.
        ShipName.text = base.GetLanguageText($"S{(int)ship}");

        // Aktif gemi miktarı.
        UserPlanetShipDTO currentShipCount = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == ship);

        // Resmi yüklüyoruz.
        ShipImage.sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == ship).ShipImage;

        // Üretim var mı?
        UserPlanetShipProgDTO prog = LoginController.LC.CurrentUser.UserPlanetShipProgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == ship);

        // Eğer üretim var ise süreyi basıyoruz. Ancak basmadan önce kontrol ediyoruz gezegendeki şuan üretimi yapılan gemi bu gemi mi?
        if (prog != null && ReferenceEquals(prog, LoginController.LC.CurrentUser.UserPlanetShipProgs.FirstOrDefault(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId)))
        {

            // İkonu ve kalan süreyi basıyoruz.
            if (!CountdownImage.gameObject.activeSelf)
                CountdownImage.gameObject.SetActive(true);

            // Tersanesini buluyoruz.
            UserPlanetBuildingDTO shipyard = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

            // Bir geminin üretimi için gereken süre.
            double countdownOneItem = DataController.DC.CalculateShipCountdown(ship, shipyard == null ? 0 : shipyard.BuildingLevel);

            // Birim başına baktıktan sonra tamamlanmasına kalan süreye bakıyoruz.
            DateTime completeTime = prog.LastVerifyDate.Value.AddSeconds(-prog.OffsetTime).AddSeconds(countdownOneItem);

            // Tamamlanmasına kalan süre.
            TimeSpan leftTime = completeTime - DateTime.UtcNow;

            // Üretim geri sayımını aktif ediyoruz.
            CountdownText.text = $"({prog.ShipCount}){Environment.NewLine}{TimeExtends.GetCountdownText(leftTime)}";

            // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
            ShipCount.text = currentShipCount == null ? $"{0}" : $"{currentShipCount.ShipCount}".ToString();
        }
        else
        {
            // İkonu kapatıyoruz.
            if (CountdownImage.gameObject.activeSelf)
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
