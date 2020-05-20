using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;
using Microsoft.EntityFrameworkCore;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public class UserController : MainController
    {
        public UserController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
        {
        }

        [HttpPost("Login")]
        [Description("Kullanıcının datalarını döner.")]
        public ApiResult Login()
        {
            TblUsers userAndHisPlanets = base.UnitOfWork.GetRepository<TblUsers>().Where(x => x.UserId == DBUser.UserId)
                .Include(x => x.TblUserResearches)
                .Include(x=> x.TblUserResearchUpgs)
                .Include(x => x.TblUserPlanets)
                .ThenInclude(x => x.TblUserPlanetBuildings)
                .Include(x => x.TblUserPlanets)
                .ThenInclude(x => x.TblUserPlanetBuildingUpgs)
                .Include(x => x.TblUserPlanets)
                .ThenInclude(x => x.TblUserPlanetDefenses)
                .Include(x => x.TblUserPlanets)
                .ThenInclude(x => x.TblUserPlanetDefenseProgs)
                .Include(x => x.TblUserPlanets)
                .ThenInclude(x => x.TblUserPlanetShips)
                .Include(x => x.TblUserPlanets)
                .ThenInclude(x => x.TblUserPlanetShipProgs).FirstOrDefault();

            // Her bir gezegenin Verify işlemini yapıyoruz.
            foreach (TblUserPlanets userPlanet in userAndHisPlanets.TblUserPlanets)
                VerifyController.VerifyPlanetResources(this, new VerifyResourceDTO { UserPlanetID = userPlanet.UserPlanetId });

            // Kullanıcıyı dönüyoruz.
            UserDTO user = new UserDTO
            {
                UserToken = DBUser.UserToken,
                CreateDate = DBUser.CreateDate,
                GoogleToken = DBUser.GoogleToken,
                IosToken = DBUser.IosToken,
                IsBanned = DBUser.IsBanned,
                UserId = DBUser.UserId,
                UserLanguage = DBUser.UserLanguage,
                Username = DBUser.Username
            };

            // Kullanıcının gezegenlerine ait binalar.
            List<UserPlanetBuildingDTO> userPlanetsBuildings = userAndHisPlanets.TblUserPlanets.SelectMany(y => y.TblUserPlanetBuildings)
                .Select(x => new UserPlanetBuildingDTO
                {
                    BuildingId = x.BuildingId,
                    BuildingLevel = x.BuildingLevel,
                    UserPlanetId = x.UserPlanetId
                }).ToList();

            // Kullanıcıya ait bütün yükseltmeler.
            List<UserPlanetBuildingUpgDTO> userPlanetBuildingUpgs = userAndHisPlanets.TblUserPlanets.SelectMany(y => y.TblUserPlanetBuildingUpgs)
                .Select(x => new UserPlanetBuildingUpgDTO
                {
                    BuildingId = x.BuildingId,
                    BuildingLevel = x.BuildingLevel,
                    LeftTime = (x.EndDate - base.RequestDate).TotalSeconds,
                    UserPlanetId = x.UserPlanetId
                })
                .ToList();

            // Kullanıcının araştırmaları.
            List<UserResearchesDTO> userResearches = userAndHisPlanets.TblUserResearches.Select(x => new UserResearchesDTO
            {
                ResearchID = x.ResearchId,
                ResearchLevel = x.ResearchLevel
            }).ToList();

            // Devam eden araştırmaları alıyoruz.
            List<UserResearchProgDTO> userResearchProgs = userAndHisPlanets.TblUserResearchUpgs.Select(x => new UserResearchProgDTO
            {
                LeftTime = (x.EndDate - base.RequestDate).TotalSeconds,
                ResearchID = x.ResearchId,
                ResearchLevel = x.ResearchTargetLevel,
            }).ToList();

            // Kullanıcının gezegenlerini buluyoruz.
            List<UserPlanetDTO> userPlanets = userAndHisPlanets.TblUserPlanets.Select(x => new UserPlanetDTO
            {
                UserId = x.UserId,
                Boron = x.Boron,
                Crystal = x.Crystal,
                Metal = x.Metal,
                PlanetName = x.PlanetName,
                PlanetType = x.PlanetType,
                UserPlanetId = x.UserPlanetId
            }).ToList();

            // Kullanıcının gemilerini buluyoruz.
            List<UserPlanetShipDTO> userPlanetShips = userAndHisPlanets.TblUserPlanets.SelectMany(x => x.TblUserPlanetShips)
                .Select(x => new UserPlanetShipDTO
                {
                    ShipCount = x.ShipCount,
                    ShipId = x.ShipId,
                    UserPlanetId = x.UserPlanetId
                }).ToList();

            // Kullanıcının gemi üretimlerini buluyoruz.
            List<UserPlanetShipProgDTO> userPlanetShipProgs = userAndHisPlanets.TblUserPlanets.SelectMany(x => x.TblUserPlanetShipProgs)
                .Select(x => new UserPlanetShipProgDTO
                {
                    ShipCount = x.ShipCount,
                    ShipId = x.ShipId,
                    UserPlanetId = x.UserPlanetId,
                    LastVerifyDate = x.LastVerifyDate
                }).ToList();

            // Farkı hesaplıyoruz.
            foreach (UserPlanetShipProgDTO shipProg in userPlanetShipProgs)
            {
                if (shipProg.LastVerifyDate.HasValue)
                {
                    double passedSeconds = (base.RequestDate - shipProg.LastVerifyDate.Value).TotalSeconds;
                    shipProg.OffsetTime = passedSeconds;
                }
            }

            // Kullanıcının savunmalarını buluyoruz.
            List<UserPlanetDefenseDTO> userPlanetDefenses = userAndHisPlanets.TblUserPlanets.SelectMany(x => x.TblUserPlanetDefenses)
                .Select(x => new UserPlanetDefenseDTO
                {
                    DefenseCount = x.DefenseCount,
                    DefenseId = x.DefenseId,
                    UserPlanetId = x.UserPlanetId
                }).ToList();

            // Kullanıcının savunma üretimlerini buluyoruz.
            List<UserPlanetDefenseProgDTO> userPlanetDefenseProgs = userAndHisPlanets.TblUserPlanets.SelectMany(x => x.TblUserPlanetDefenseProgs)
                .Select(x => new UserPlanetDefenseProgDTO
                {
                    DefenseCount = x.DefenseCount,
                    DefenseId = x.DefenseId,
                    UserPlanetId = x.UserPlanetId,
                    LastVerifyDate = x.LastVerifyDate
                }).ToList();

            // Farkı hesaplıyoruz.
            foreach (UserPlanetDefenseProgDTO defenseProg in userPlanetDefenseProgs)
            {
                if (defenseProg.LastVerifyDate.HasValue)
                {
                    double passedSeconds = (base.RequestDate - defenseProg.LastVerifyDate.Value).TotalSeconds;
                    defenseProg.OffsetTime = passedSeconds;
                }
            }

            // Kordinatları alıyoruz.
            List<UserPlanetCordinatesDTO> cordinates = base.UnitOfWork.GetRepository<TblCordinates>()
                .Where(x => userPlanets.Select(y => y.UserPlanetId).Contains(x.UserPlanetId.Value))
                .Select(x => new UserPlanetCordinatesDTO
                {
                    GalaxyIndex = x.GalaxyIndex,
                    OrderIndex = x.OrderIndex,
                    SolarIndex = x.SolarIndex,
                    UserPlanetId = x.UserPlanetId.Value
                }).ToList();

            // Sonucu dönüyoruz.
            return ResponseHelper.GetSuccess(new LoginStuffDTO
            {
                UserData = user,
                UserPlanetsBuildings = userPlanetsBuildings,
                UserPlanetsBuildingsUpgs = userPlanetBuildingUpgs,
                UserPlanets = userPlanets,
                UserResearches = userResearches,
                UserResearchProgs = userResearchProgs,
                UserPlanetShips = userPlanetShips,
                UserPlanetShipProgs = userPlanetShipProgs,
                UserPlanetDefenseProgs = userPlanetDefenseProgs,
                UserPlanetDefenses = userPlanetDefenses,
                UserPlanetCordinates = cordinates
            });
        }

        [HttpPost("VerifyUserData")]
        [Description("Kullanıcının bütün datalarını doğrular.")]
        public ApiResult VerifyUserData(VerifyResourceDTO verify)
        {
            // Her bir gezegenin Verify işlemini yapıyoruz.
            VerifyController.VerifyPlanetResources(this, new VerifyResourceDTO { UserPlanetID = verify.UserPlanetID });

            // Kullanıcının gezegenlerini buluyoruz.
            UserPlanetDTO userPlanet = this.UnitOfWork.GetRepository<TblUserPlanets>().Where(x => x.UserId == DBUser.UserId)
                .Select(x => new UserPlanetDTO
                {
                    UserId = x.UserId,
                    Boron = x.Boron,
                    Crystal = x.Crystal,
                    Metal = x.Metal,
                    PlanetName = x.PlanetName,
                    PlanetType = x.PlanetType,
                    UserPlanetId = x.UserPlanetId
                }).FirstOrDefault(x => x.UserPlanetId == verify.UserPlanetID);


            // Eğer tamamlandıysa geri dön.
            return ResponseHelper.GetSuccess(userPlanet);
        }

    }
}