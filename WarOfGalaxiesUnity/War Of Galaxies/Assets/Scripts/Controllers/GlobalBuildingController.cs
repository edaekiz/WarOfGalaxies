using Assets.Scripts.ApiModels;
using Assets.Scripts.Models;
using System.Collections.Generic;
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

    [Header("Binaların seviyeleri ve diğer bilgilerini tutar.")]
    public List<BuildingLevelDTO> BuildingLevels;

    [Header("Kullanıcının gezegenlerdeki binaları.")]
    public List<UserPlanetBuildingDTO> UserPlanetBuildings;

    [Header("Kullanıcının gezegenlerdeki binaların devan eden yükseltmeleri.")]
    public List<UserPlanetBuildingUpgDTO> UserPlanetBuildingsUpgs;

    void Start()
    {
        LoadBuildingLevels();
        LoadUserBuildings();
        LoadUserBuildingsProgs();
    }


    public void LoadBuildingLevels()
    {
        StartCoroutine(ApiService.API.Post("GetBuildingLevels", null, (ApiResult response) =>
        {
            if (response.IsSuccess)
            {
                // Bina seviyelerini yüklüyoruz.
                BuildingLevels = response.GetDataList<BuildingLevelDTO>();

                // Yüklemeyi ilerlet.
                LoadingController.LC.IncreaseLoadCount();
            }
        }));
    }

    public void LoadUserBuildings()
    {
        StartCoroutine(ApiService.API.Post("GetUserBuildings", null, (ApiResult response) =>
        {
            if (response.IsSuccess)
            {
                // Kullanıcının gezegenlerindeki binlaar
                UserPlanetBuildings = response.GetDataList<UserPlanetBuildingDTO>();

                // Yüklemeyi ilerlet.
                LoadingController.LC.IncreaseLoadCount();
            }
        }));
    }

    public void LoadUserBuildingsProgs()
    {
        StartCoroutine(ApiService.API.Post("GetUserBuildingsProgs", null, (ApiResult response) =>
        {
            if (response.IsSuccess)
            {
                // Bina yükseltmeleri.
                UserPlanetBuildingsUpgs = response.GetDataList<UserPlanetBuildingUpgDTO>();

                // Yüklemeyi ilerlet.
                LoadingController.LC.IncreaseLoadCount();
            }
        }));
    }


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
