using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
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

    private void Start()
    {
        InvokeRepeating("RefreshState", 0, 1);
    }

    public void LoadShipDetails(Ships ship)
    {
        // Gemi bilgisi.
        CurrentShip = ship;

        // Gemi ismi.
        ShipName.text = base.GetLanguageText($"S{(int)ship}");

        // Gemi resim bilgisi.
        ShipImageDTO shipImage = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == ship);

        // Resmi yüklüyoruz.
        if (shipImage != null)
            ShipImage.sprite = shipImage.ShipImage;
    }

    public void RefreshState()
    {
        // Aktif gemi miktarı.
        UserPlanetShipDTO currentShipCount = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == CurrentShip);

        // Üretim var mı?
        UserPlanetShipProgDTO prog = LoginController.LC.CurrentUser.UserPlanetShipProgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == CurrentShip);

        // Eğer üretim var ise süreyi basıyoruz. Ancak basmadan önce kontrol ediyoruz gezegendeki şuan üretimi yapılan gemi bu gemi mi?
        if (prog != null && ReferenceEquals(prog, LoginController.LC.CurrentUser.UserPlanetShipProgs.FirstOrDefault(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId)))
        {
            // İkonu ve kalan süreyi basıyoruz.
            if (!CountdownImage.gameObject.activeSelf)
                CountdownImage.gameObject.SetActive(true);

            // Tersanesini buluyoruz.
            UserPlanetBuildingDTO shipyard = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

            // Bir geminin üretimi için gereken süre.
            double countdownOneItem = DataController.DC.CalculateShipCountdown(CurrentShip, shipyard == null ? 0 : shipyard.BuildingLevel);

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
    }

    public void ShowShipDetail()
    {
        GameObject shipyardDetailPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.ShipyardDetailPanel);
        ShipyardDetailItemPanel sdip = shipyardDetailPanel.GetComponent<ShipyardDetailItemPanel>();
        sdip.LoadShipDetals(CurrentShip);
    }

}
