using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
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

    internal void LoadShipDetails(Ships ship)
    {
        CurrentShip = ship;

        UserPlanetShipDTO currentShipCount = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == ship);

        ShipCount.text = currentShipCount == null ? "0" : currentShipCount.ShipCount.ToString();
    }

    public void ShowShipDetail()
    {
        GameObject shipyardDetailPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.ShipyardDetailPanel);
        shipyardDetailPanel.GetComponent<ShipyardDetailItemPanel>().LoadShipDetals(CurrentShip);


    }

}
