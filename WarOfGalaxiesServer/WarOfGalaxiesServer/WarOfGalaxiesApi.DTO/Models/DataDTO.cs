using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class DataDTO
    {
        public List<BuildingDataDTO> Buildings { get; set; }
        public List<DefenseDataDTO> Defenses { get; set; }
        public List<ShipDataDTO> Ships { get; set; }
        public List<ResearchDataDTO> Researches { get; set; }
        public List<ParameterDTO> Parameters { get; set; }
    }

    public class BuildingDataDTO
    {
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public double BaseCostMetal { get; set; }
        public double BaseCostCrystal { get; set; }
        public double BaseCostBoron { get; set; }
        public double BaseValue { get; set; }
        public double BuildingUpgradeCostRate { get; set; }
    }
    public class DefenseDataDTO
    {
        public int DefenseId { get; set; }
        public string DefenseName { get; set; }
        public double CostMetal { get; set; }
        public double CostCrystal { get; set; }
        public double CostBoron { get; set; }
    }
    public class ShipDataDTO
    {
        public int ShipId { get; set; }
        public string ShipName { get; set; }
        public double CostMetal { get; set; }
        public double CostCrystal { get; set; }
        public double CostBoron { get; set; }
        public double ShipSpeed { get; set; }
        public int ShipFuelt { get; set; }
        public int CargoCapacity { get; set; }
    }
    public class ResearchDataDTO
    {
        public int ResearchId { get; set; }
        public string ResearchName { get; set; }
        public double BaseCostMetal { get; set; }
        public double BaseCostCrystal { get; set; }
        public double BaseCostBoron { get; set; }
        public double ResearchValue { get; set; }
    }
    public class ParameterDTO
    {
        public int ParameterId { get; set; }
        public string Description { get; set; }
        public int? ParameterIntValue { get; set; }
        public double? ParameterFloatValue { get; set; }
        public DateTime? ParameterDateTimeValue { get; set; }
        public bool? ParameterBitValue { get; set; }
    }
}
