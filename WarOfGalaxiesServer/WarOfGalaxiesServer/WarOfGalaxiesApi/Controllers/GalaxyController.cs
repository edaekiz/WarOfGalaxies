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
            List<TblCordinates> cordinates = base.UnitOfWork.GetRepository<TblCordinates>()
                .Where(x => x.GalaxyIndex == request.GalaxyIndex && x.SolarIndex == request.SolarIndex)
                .ToList();

            // Gezegenlerin listesini alıyoruz.
            int[] userPlanetIds = cordinates.Where(x => x.UserPlanetId.HasValue).Select(x => x.UserPlanetId.Value).ToArray();

            // Kullanıcıların gezegenlerini alıyoruz.
            List<UserPlanetDTO> userPlanets = base.UnitOfWork.GetRepository<TblUserPlanets>()
                .Where(x => userPlanetIds.Contains(x.UserPlanetId))
                .Select(x => new UserPlanetDTO
                {
                    UserPlanetId = x.UserPlanetId,
                    UserId = x.UserId,
                    PlanetName = x.PlanetName,
                    PlanetType = x.PlanetType
                }).ToList();

            // Yanıtı oluşturuyoruz.
            GalaxyInfoResponseDTO response = new GalaxyInfoResponseDTO
            {
                GalaxyIndex = request.GalaxyIndex,
                SolarIndex = request.SolarIndex
            };

            // Her bir kordinatı dönüyoruz. Eğer var ise gezegen bilgisini yüklüyoruz.
            foreach (TblCordinates cordinate in cordinates)
            {
                SolarPlanetDTO solarPlanet = new SolarPlanetDTO();
                if (cordinate.UserPlanetId.HasValue)
                    solarPlanet.UserPlanet = userPlanets.Find(x => x.UserPlanetId == cordinate.UserPlanetId);
                solarPlanet.OrderIndex = cordinate.OrderIndex;
                response.SolarPlanets.Add(solarPlanet);
            }

            // Sonucu dönüyoruz.
            return ResponseHelper.GetSuccess(response);
        }

    }
}
