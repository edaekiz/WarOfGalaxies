using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public class TechnologyController : MainController
    {
        public TechnologyController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
        {
        }

        [HttpPost("GetTechnologyTree")]
        [Description("Teknoloji ağacını döner.")]
        public ApiResult GetTechnologyTree()
        {
            return ResponseHelper.GetSuccess(StaticValues.DbTechnologies.Select(x => new TechnologyDTO
            {
                IndexId = x.IndexId,
                RequiredIndexId = x.RequiredIndexId,
                RequiredLevel = x.RequiredLevel,
                RequiredTechnologyCategoryId = x.RequiredTechnologyCategoryId,
                TechnologyCategoryId = x.TechnologyCategoryId
            }).ToList());
        }

    }
}
