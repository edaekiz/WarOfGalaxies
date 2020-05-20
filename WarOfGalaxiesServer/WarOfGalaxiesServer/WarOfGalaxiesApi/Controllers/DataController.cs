using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public class DataController : MainController
    {
        public DataController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
        {
        }

        [HttpPost("GetSystemData")]
        [Description("Sistem datalarını dönüyoruz.")]
        public ApiResult GetSystemData()
        {
            List<BuildingDataDTO> buildings = base.StaticValues.DBBuildings.Select(x => new BuildingDataDTO
            {
                BaseCostBoron = x.BaseCostBoron,
                BaseCostCrystal = x.BaseCostCrystal,
                BaseCostMetal = x.BaseCostMetal,
                BaseValue = x.BaseValue,
                BuildingId = x.BuildingId,
                BuildingName = x.BuildingName,
                BuildingUpgradeCostRate = x.BuildingUpgradeCostRate
            }).ToList();

            List<DefenseDataDTO> defenses = base.StaticValues.DBDefenses.Select(x => new DefenseDataDTO
            {
                CostBoron = x.CostBoron,
                CostCrystal = x.CostCrystal,
                CostMetal = x.CostMetal,
                DefenseId = x.DefenseId
            }).ToList();

            List<ShipDataDTO> ships = base.StaticValues.DBShips.Select(x => new ShipDataDTO
            {
                CostCrystal = x.CostCrystal,
                CostMetal = x.CostMetal,
                CostBoron = x.CostBoron,
                CargoCapacity = x.CargoCapacity,
                ShipFuelt = x.ShipFuelt,
                ShipId = x.ShipId,
                ShipName = x.ShipName,
                ShipSpeed = x.ShipSpeed
            }).ToList();

            List<ResearchDataDTO> researches = base.StaticValues.DbResearches.Select(x => new ResearchDataDTO
            {
                BaseCostBoron = x.BaseCostBoron,
                BaseCostCrystal = x.BaseCostCrystal,
                BaseCostMetal = x.BaseCostMetal,
                ResearchId = x.ResearchId,
                ResearchName = x.ResearchName,
                ResearchValue = x.ResearchValue
            }).ToList();

            List<ParameterDTO> parameters = base.StaticValues.DBParameters.Where(x => x.ParameterSendToUserValue).Select(x => new ParameterDTO
            {
                Description = x.Description,
                ParameterBitValue = x.ParameterBitValue,
                ParameterDateTimeValue = x.ParameterDateTimeValue,
                ParameterFloatValue = x.ParameterFloatValue,
                ParameterId = x.ParameterId,
                ParameterIntValue = x.ParameterIntValue
            }).ToList();

            return ResponseHelper.GetSuccess(new DataDTO
            {
                Buildings = buildings,
                Defenses = defenses,
                Ships = ships,
                Researches = researches,
                Parameters = parameters
            });
        }
    }
}
