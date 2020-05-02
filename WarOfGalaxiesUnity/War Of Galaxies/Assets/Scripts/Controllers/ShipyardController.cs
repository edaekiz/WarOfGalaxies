using UnityEngine;

public class ShipyardController : MonoBehaviour
{
    public static ShipyardController SC { get; set; }

    private void Awake()
    {
        if (SC == null)
            SC = this;
        else
            Destroy(gameObject);
    }

    
    public void ShowShipyardPanel()
    {
        GameObject panel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.ShipyardPanel);
        panel.GetComponent<ShipyardPanelController>().LoadAllShips();
    }

}
