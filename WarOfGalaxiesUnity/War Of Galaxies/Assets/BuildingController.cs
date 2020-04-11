using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [Header("Seçim yapıldığında açılacak olan obje.")]
    public GameObject SelectionObject;

    private void OnMouseDown()
    {
        // Eğer zaten bir panel açık ise geri dön.
        if (GlobalPanelController.GPC.IsAnyPanelOpen)
            return;

        // Binayı seçiyoruz.
        GlobalBuildingController.GBC.SelectBuilding(this);

        // Paneli gösteriyoruz.
        GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.BuildingPanel);
    }
}
