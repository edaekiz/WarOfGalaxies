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
                BuildingLevels = response.GetDataList<BuildingLevelDTO>();
        }));
    }

    public void LoadUserBuildings()
    {
        StartCoroutine(ApiService.API.Post("GetUserBuildings", null, (ApiResult response) =>
        {
            if (response.IsSuccess)
                UserPlanetBuildings = response.GetDataList<UserPlanetBuildingDTO>();
        }));
    }

    public void LoadUserBuildingsProgs()
    {
        StartCoroutine(ApiService.API.Post("GetUserBuildingsProgs", null, (ApiResult response) =>
        {
            if (response.IsSuccess)
                UserPlanetBuildingsUpgs = response.GetDataList<UserPlanetBuildingUpgDTO>();
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
