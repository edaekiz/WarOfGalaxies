using Assets.Scripts.Enums;
using System;
using UnityEngine;

public class ShipyardPanelController : BasePanelController
{
    /// <summary>
    /// Açık olan araştırma paneli
    /// </summary>
    public static ShipyardPanelController SPC { get; set; }

    [Header("Basılacak olan gemiler.")]
    public GameObject ShipyardItem;

    [Header("Gemileri buraya basacağız.")]
    public Transform ShipyardItemContent;

    private void Awake()
    {
        if (SPC == null)
            SPC = this;
        else
            Destroy(gameObject);
    }

    public void LoadAllShips()
    {
        Array ships = Enum.GetValues(typeof(Ships));


        for (int ii = 0; ii < ships.Length; ii++)
        {
            Ships ship = (Ships)ships.GetValue(ii);

            GameObject shipyardItem = Instantiate(ShipyardItem, ShipyardItemContent);

            shipyardItem.GetComponent<ShipyardItemController>().LoadShipDetails(ship);

        }

    }

}
