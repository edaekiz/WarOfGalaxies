using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;

namespace WarOfGalaxiesApi.Controllers
{
    public class GalaxyController : MainController
    {
        public GalaxyController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpPost("GetCordinateDetails")]
        [Description("Verilen kordinate ait detayları yükler.")]
        public ApiResult GetCordinateDetails(GalaxyInfoRequestDTO request)
        {
            // Kordinatları ve datalarını alıyoruz.
            List<SolarPlanetDTO> cordinates = base.UnitOfWork.GetRepository<TblCordinates>()
                .Where(x => x.GalaxyIndex == request.GalaxyIndex && x.SolarIndex == request.SolarIndex)
                .Select(x => new SolarPlanetDTO
                {
                    OrderIndex = x.OrderIndex,
                    UserPlanet = x.UserPlanetId.HasValue ? new UserPlanetDTO
                    {
                        PlanetName = x.UserPlanet.PlanetName,
                        UserId = x.UserPlanet.UserId,
                        UserPlanetId = x.UserPlanet.UserPlanetId,
                        PlanetType = x.UserPlanet.PlanetType
                    } : null
                })
                .ToList();

            // Sonucu dönüyoruz.
            return ResponseHelper.GetSuccess(new GalaxyInfoResponseDTO
            {
                GalaxyIndex = request.GalaxyIndex,
                SolarIndex = request.SolarIndex,
                SolarPlanets = cordinates
            });
        }

    }
}
