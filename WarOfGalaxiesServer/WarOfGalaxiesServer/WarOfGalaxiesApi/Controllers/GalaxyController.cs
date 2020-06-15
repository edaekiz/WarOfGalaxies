using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public class GalaxyController : MainController
    {
        public GalaxyController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
        {
        }

        [HttpPost("GetCordinateDetails")]
        [Description("Verilen kordinate ait detayları yükler.")]
        public ApiResult GetCordinateDetails(GalaxyInfoRequestDTO request)
        {
            // Kordinatları ve datalarını alıyoruz.
            List<TblCordinates> cordinates = base.UnitOfWork.GetRepository<TblCordinates>()
                .Where(x => x.GalaxyIndex == request.GalaxyIndex && x.SolarIndex == request.SolarIndex)
                .Include(x => x.UserPlanet)
                .ToList();

            // Kordinatlardaki gezegenleri alıyoruz.
            int[] userPlanetIds = cordinates.Where(x => x.UserPlanet != null).Select(y => y.UserPlanet.UserPlanetId).ToArray();

            // Eğer bir gezegen yok ise onaya gerek yok.
            if (userPlanetIds.Length > 0)
            {
                // Bunlardan verify edilmesi gerekenler var mı diye bakıyoruz.
                // Nasıl anlıyoruz. Sadece saldırı ve Sök hareketleri var ise verify gerekecek.
                // Aynı zamanda verify etmeden önce kontrol etmeliyiz bu filo hareketi hedefe ulaştı mı?
                // Ve tabiki dönen filoların bir önemi yok yalnızca hedefe ulaşan filolar lazım bize.
                int[] validationRequiredPlanetIds = base.UnitOfWork.GetRepository<TblFleets>()
                    .Where(x => x.EndDate <= RequestDate)
                    .Where(x => x.FleetActionTypeId == (int)FleetTypes.Saldır || x.FleetActionTypeId == (int)FleetTypes.Sök)
                    .Where(x => x.ReturnFleetId.HasValue)
                    .Where(x => x.SenderUserPlanetId.HasValue)
                    .Where(x => userPlanetIds.Contains(x.SenderUserPlanetId.Value))
                    .Select(x => x.SenderUserPlanetId.Value)
                    .ToArray();

                // Bulunan gezegenleri onaylıyoruz.
                foreach (int userPlanetId in validationRequiredPlanetIds)
                    VerifyController.VerifyAllFleets(this, new VerifyResourceDTO { UserPlanetID = userPlanetId, VerifyDate = RequestDate });
            }

            // Kordinatları oluşturuyoruz.
            List<SolarPlanetDTO> cordinatesDTO = cordinates.Select(x => new SolarPlanetDTO
            {
                OrderIndex = x.OrderIndex,
                UserPlanet = x.UserPlanetId.HasValue ? new UserPlanetDTO
                {
                    PlanetName = x.UserPlanet.PlanetName,
                    UserId = x.UserPlanet.UserId,
                    UserPlanetId = x.UserPlanet.UserPlanetId,
                    PlanetType = x.UserPlanet.PlanetType
                } : null,
                GarbageMetal = x.Metal,
                GarbageCrystal = x.Crystal,
                GarbageBoron = x.Boron
            }).ToList();

            // Sonucu dönüyoruz.
            return ResponseHelper.GetSuccess(new GalaxyInfoResponseDTO
            {
                GalaxyIndex = request.GalaxyIndex,
                SolarIndex = request.SolarIndex,
                SolarPlanets = cordinatesDTO
            });
        }

    }
}
