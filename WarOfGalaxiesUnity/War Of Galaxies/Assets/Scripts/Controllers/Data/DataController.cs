using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController DC { get; set; }
    public DataDTO SystemData { get; set; }
    private void Awake()
    {
        if (DC == null)
            DC = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSystemData();
    }

    public void LoadSystemData()
    {
        ApiService.API.Post("GetSystemData", null, (ApiResult response) =>
        {
            if (response.IsSuccess)
            {
                SystemData = response.GetData<DataDTO>();
                LoadingController.LC.IncreaseLoadCount();
            }
        });
    }

    #region Shorts

    public double UniverseSpeed => SystemData.Parameters.Find(x => x.ParameterId == (int)ParameterTypes.UniverseSpeed).ParameterFloatValue;
    public double UniverseFleetSpeed => GetParameter(ParameterTypes.UniverseFleetSpeed).ParameterFloatValue;
    public double UniverseResearchSpeed => GetParameter(ParameterTypes.UniverseResearchSpeed).ParameterFloatValue;

    #endregion

    #region Formulas

    #region Buildings

    public ResourcesDTO CalculateCostBuilding(Buildings building, int buildingLevel)
    {
        BuildingDataDTO baseCost = GetBuilding(building);
        ResourcesDTO cost = new ResourcesDTO(baseCost.BaseCostMetal, baseCost.BaseCostCrystal, baseCost.BaseCostBoron);
        return cost * Math.Pow(baseCost.BuildingUpgradeCostRate, buildingLevel);
    }

    public double CalculateBuildingUpgradeTime(Buildings building, int buildingLevel, int robotFactoryLevel)
    {
        ResourcesDTO buildingCost = CalculateCostBuilding(building, buildingLevel);
        return ((buildingCost.Metal + buildingCost.Crystal) / ((double)2500 * (1 + robotFactoryLevel) * UniverseSpeed)) * 3600;
    }

    public double GetBuildingProdPerHour(Buildings building, int buildingLevel)
    {
        BuildingDataDTO buildingInfo = GetBuilding(building);
        return buildingInfo.BaseValue * UniverseSpeed * buildingLevel * Math.Pow(1.1f, buildingLevel) + buildingInfo.BaseValue * UniverseSpeed;
    }

    public double GetBuildingStorage(Buildings building, int buildingLevel)
    {
        BuildingDataDTO buildingInfo = GetBuilding(building);
        return buildingInfo.BaseValue + 50000 * (Math.Pow(1.6f, buildingLevel) - 1);
    }

    #endregion

    #region Researches

    public ResourcesDTO CalculateCostResearch(Researches research, int researchLevel)
    {
        ResearchDataDTO researchItem = GetResearches(research);
        return new ResourcesDTO(Math.Pow(2, researchLevel) * researchItem.BaseCostMetal, Math.Pow(2, researchLevel) * researchItem.BaseCostCrystal, Math.Pow(2, researchLevel) * researchItem.BaseCostBoron);
    }

    public double CalculateResearchUpgradeTime(Researches research, int totalResearchLevel, int researchLevel)
    {
        ResourcesDTO cost = CalculateCostResearch(research, researchLevel);
        return ((cost.Metal + cost.Crystal) / (UniverseResearchSpeed * 1000 * (1 + totalResearchLevel))) * 3600;
    }

    public double CalculateResearchUpgradeTime(ResourcesDTO cost, int totalResearchLevel)
    {
        return ((cost.Metal + cost.Crystal) / (UniverseResearchSpeed * 1000 * (1 + totalResearchLevel))) * 3600;
    }

    #endregion

    #region Ships

    public double CalculateShipCountdown(Ships ship, int shipyardLevel)
    {
        ShipDataDTO shipInfo = GetShip(ship);
        return ((shipInfo.CostMetal + shipInfo.CostCrystal) / (2500 * (10 + shipyardLevel))) * 3600;
    }

    #endregion

    #region Defenses

    public double CalculateDefenseCountdown(Defenses defense, int robotLevel)
    {
        DefenseDataDTO defenseInfo = GetDefense(defense);
        return ((defenseInfo.CostMetal + defenseInfo.CostCrystal) / (2500 * (10 + robotLevel))) * 3600;
    }

    #endregion

    #endregion

    #region DB Loads

    #region Buiildings

    public BuildingDataDTO GetBuilding(Buildings building) => SystemData.Buildings.Find(x => x.BuildingId == (int)building);

    #endregion

    #region Defenses

    public DefenseDataDTO GetDefense(Defenses defense) => SystemData.Defenses.Find(x => x.DefenseId == (int)defense);

    #endregion

    #region Ships

    public ShipDataDTO GetShip(Ships ship) => SystemData.Ships.Find(x => x.ShipId == (int)ship);

    #endregion

    #region Researches

    public ResearchDataDTO GetResearches(Researches research) => SystemData.Researches.Find(x => x.ResearchId == (int)research);

    #endregion

    #region Parameters

    public ParameterDataDTO GetParameter(ParameterTypes parameter) => SystemData.Parameters.Find(x => x.ParameterId == (int)parameter);

    #endregion

    #endregion


}
