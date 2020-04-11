using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [Header("Seçim yapıldığında açılacak olan mesh.")]
    public GameObject SelectionMesh;

    [Header("Binanın kurulu olduğunda basılacak olan meshi.")]
    public GameObject BuildingMesh;

    [Header("Binaya sahip olmadığında görüntülenecek olan mesh.")]
    public GameObject ConstructableMesh;

    private void Start()
    {
        // Seçim başlangıç da kalkıyor.
        SelectionMesh.SetActive(false);

        // Binayı kapatıyoruz.
        BuildingMesh.SetActive(false);

        // İnşaa edilebilir olduğunu söylüyoruz.
        ConstructableMesh.SetActive(true);
    }

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
