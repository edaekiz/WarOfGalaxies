using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
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

    [Header("Keşfedilmemiş gemilerin rengi")]
    public Color32 NotInventedItemColor;

    [Header("Keşfedilmemiş gemilerin üstünde olacak ikon.")]
    public GameObject LockedIcon;

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
        
        // Aktif gemi miktarı.
        UserPlanetShipDTO currentShipCount = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == CurrentShip);

        // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
        ShipCount.text = currentShipCount == null ? $"{0}" : $"{currentShipCount.ShipCount}";

        // Keşfedilmedi ise keşfedilmedi uyarısını çıkaraağız.
        if (!TechnologyController.TC.IsInvented(TechnologyCategories.Gemiler, (int)ship))
        {
            // Disabled rengine boyuyoruz.
            GetComponent<Image>().color = NotInventedItemColor;

            // Disabled rengine boyuyoruz gemiyi.
            ShipImage.color = NotInventedItemColor;

            // Kilitli ikonunu açıyoruz.
            LockedIcon.SetActive(true);
        }

    }

    public void ShowShipDetail()
    {
        GameObject shipyardDetailPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.ShipyardDetailPanel);
        ShipyardDetailItemPanel sdip = shipyardDetailPanel.GetComponent<ShipyardDetailItemPanel>();
        sdip.LoadShipDetals(CurrentShip);
    }

}
