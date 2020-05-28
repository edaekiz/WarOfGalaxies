using System;
using System.Collections.Generic;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class DataDTO
    {
        public List<BuildingDataDTO> Buildings;
        public List<DefenseDataDTO> Defenses;
        public List<ShipDataDTO> Ships;
        public List<ResearchDataDTO> Researches;
        public List<ParameterDataDTO> Parameters;
        public DataDTO()
        {
            Buildings = new List<BuildingDataDTO>();
            Defenses = new List<DefenseDataDTO>();
            Ships = new List<ShipDataDTO>();
            Researches = new List<ResearchDataDTO>();
            Parameters = new List<ParameterDataDTO>();
        }
    }

    [Serializable]
    public class BuildingDataDTO
    {
        public int BuildingId;
        public string BuildingName;
        public double BaseCostMetal;
        public double BaseCostCrystal;
        public double BaseCostBoron;
        public double BaseValue;
        public double BuildingUpgradeCostRate;
    }

    [Serializable]
    public class DefenseDataDTO
    {
        public int DefenseId;
        public string DefenseName;
        public double CostMetal;
        public double CostCrystal;
        public double CostBoron;
    }

    [Serializable]
    public class ShipDataDTO
    {
        public int ShipId;
        public string ShipName;
        public double CostMetal;
        public double CostCrystal;
        public double CostBoron;
        public double ShipSpeed;
        public int ShipFuelt;
        public int CargoCapacity;
    }

    [Serializable]
    public class ResearchDataDTO
    {
        public int ResearchId;
        public string ResearchName;
        public double BaseCostMetal;
        public double BaseCostCrystal;
        public double BaseCostBoron;
        public double ResearchValue;
    }
    [Serializable]
    public class ParameterDataDTO
    {
        public int ParameterId;
        public string Description;
        public int ParameterIntValue;
        public double ParameterFloatValue;
        public DateTime ParameterDateTimeValue;
        public bool ParameterBitValue;
    }
}
