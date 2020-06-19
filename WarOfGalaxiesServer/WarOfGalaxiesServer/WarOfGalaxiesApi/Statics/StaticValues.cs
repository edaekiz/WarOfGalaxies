using System;
using System.Collections.Generic;
using System.Linq;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DAL.Repositories;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Models;

namespace WarOfGalaxiesApi.Statics
{
    public class StaticValues : IDisposable
    {
        #region Properties

        public IUnitOfWork UOW { get; set; }
        public List<TblParameters> DBParameters { get; set; }
        public List<TblShips> DBShips { get; set; }
        public List<TblDefenses> DBDefenses { get; set; }
        public List<TblBuildings> DBBuildings { get; set; }
        public List<TblResearches> DbResearches { get; set; }
        public List<TblTechnology> DbTechnologies { get; set; }
        #endregion

        #region Constructor

        public StaticValues()
        {
            UOW = new UnitOfWork();
            LoadParameters();
            LoadShips();
            LoadDefenses();
            LoadBuildings();
            LoadResearches();
            LoadTechnologies();
            UOW.Dispose();
        }

        #endregion

        #region Short Parameters

        public double UniverseSpeed => GetParameter(ParameterTypes.UniverseSpeed).ParameterFloatValue.Value;

        public double UniverseFleetSpeed => GetParameter(ParameterTypes.UniverseFleetSpeed).ParameterFloatValue.Value;

        public double UniverseResearchSpeed => GetParameter(ParameterTypes.UniverseResearchSpeed).ParameterFloatValue.Value;

        #endregion

        #region Formulas

        #region Buildings

        public ResourcesDTO CalculateCostBuilding(Buildings building, int buildingLevel)
        {
            TblBuildings baseCost = GetBuilding(building);
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
            TblBuildings buildingInfo = GetBuilding(building);
            return buildingInfo.BaseValue * UniverseSpeed * buildingLevel * Math.Pow(1.1f, buildingLevel) + buildingInfo.BaseValue * UniverseSpeed;
        }

        public double GetBuildingStorage(Buildings building, int buildingLevel)
        {
            TblBuildings buildingInfo = GetBuilding(building);
            return buildingInfo.BaseValue + 50000 * (Math.Pow(1.6f, buildingLevel) - 1);
        }

        #endregion

        #region Researches

        public ResourcesDTO CalculateCostResearch(Researches research, int researchLevel)
        {
            TblResearches researchItem = GetResearches(research);
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
            TblShips shipInfo = GetShip(ship);
            return ((shipInfo.CostMetal + shipInfo.CostCrystal) / (2500 * (10 + shipyardLevel))) * 3600;
        }

        #endregion

        #region Defenses

        public double CalculateDefenseCountdown(Defenses defense, int robotLevel)
        {
            TblDefenses defenseInfo = GetDefense(defense);
            return ((defenseInfo.CostMetal + defenseInfo.CostCrystal) / (2500 * (10 + robotLevel))) * 3600;
        }

        #endregion

        #region Cordinates

        public bool IsValidCordinate(int galaxyIndex, int solarIndex, int planetOrderIndex)
        {
            int galaxyCount = GetParameter(ParameterTypes.GalaxyCount).ParameterIntValue.Value;
            int solarSystemCount = GetParameter(ParameterTypes.SolarSystemCount).ParameterIntValue.Value;
            int planetCount = GetParameter(ParameterTypes.SolarSystemPlanetCount).ParameterIntValue.Value;
            if (galaxyIndex > galaxyCount || solarIndex > solarSystemCount || planetOrderIndex > planetCount)
                return false;
            return true;
        }

        #endregion

        #endregion

        #region DB Loads

        #region Buiildings

        public void LoadBuildings() => DBBuildings = UOW.GetRepository<TblBuildings>().ToList();

        public TblBuildings GetBuilding(Buildings building) => DBBuildings.Find(x => x.BuildingId == (int)building);

        #endregion

        #region Defenses

        public void LoadDefenses() => DBDefenses = UOW.GetRepository<TblDefenses>().ToList();

        public TblDefenses GetDefense(Defenses defense) => DBDefenses.Find(x => x.DefenseId == (int)defense);

        public IEnumerable<TblDefenses> GetDefenses(IEnumerable<Defenses> defenses) => DBDefenses.Where(x => defenses.Contains((Defenses)x.DefenseId));

        #endregion

        #region Ships

        public void LoadShips() => DBShips = UOW.GetRepository<TblShips>().ToList();

        public TblShips GetShip(Ships ship) => DBShips.Find(x => x.ShipId == (int)ship);

        public IEnumerable<TblShips> GetShips(IEnumerable<Ships> ships) => DBShips.Where(x => ships.Contains((Ships)x.ShipId));

        #endregion

        #region Researches
        public void LoadResearches() => DbResearches = UOW.GetRepository<TblResearches>().ToList();

        public TblResearches GetResearches(Researches research) => DbResearches.Find(x => x.ResearchId == (int)research);

        #endregion

        #region Parameters

        public void LoadParameters() => DBParameters = UOW.GetRepository<TblParameters>().ToList();

        public TblParameters GetParameter(ParameterTypes parameter) => DBParameters.Find(x => x.ParameterId == (int)parameter);

        #endregion

        #region Technology

        public void LoadTechnologies() => DbTechnologies = UOW.GetRepository<TblTechnology>().ToList();

        #endregion

        #endregion

        #region Dispose

        private bool isDisposed;
        public void Dispose()
        {
            if (isDisposed)
                return;

            // Veritabanını dispose ediyoruz.
            UOW.Dispose();

            isDisposed = true;
        }

        #endregion
    }
}
