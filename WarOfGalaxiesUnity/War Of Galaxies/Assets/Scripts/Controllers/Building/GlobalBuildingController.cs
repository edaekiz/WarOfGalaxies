using UnityEngine;

public class GlobalBuildingController : MonoBehaviour
{
    #region Singleton

    public static GlobalBuildingController GBC { get; set; }
    private void Awake()
    {
        if (GBC == null)
            GBC = this;
        else
            Destroy(GBC);
    }

    #endregion

    [Header("Seçili olan bina.")]
    public BuildingController CurrentSelectedBuilding;

    public void SelectBuilding(BuildingController _selectedBuilding)
    {
        // Tüm seçimleri kaldırıyoruz.
        DeSelectBuilding();

        // Binayı seçiyoruz.
        CurrentSelectedBuilding = _selectedBuilding;

        // Seçimini açıyoruz.
        CurrentSelectedBuilding.SelectionMesh.gameObject.SetActive(true);
    }

    public void DeSelectBuilding()
    {
        // Bütün seçimleri kaldırıyoruz.
        foreach (BuildingController bc in FindObjectsOfType<BuildingController>())
            bc.SelectionMesh.SetActive(false);
    }
}
