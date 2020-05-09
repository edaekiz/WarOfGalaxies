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

namespace WarOfGalaxiesApi.Controllers
{
    public class UserController : MainController
    {
        public UserController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpPost("Login")]
        [Description("Kullanıcının datalarını döner.")]
        public ApiResult Login()
        {
            // Kullanıcının gezegenlerinin idsi.
            int[] userPlanetIds = this.UnitOfWork.GetRepository<TblUserPlanets>().Where(x => x.UserId == base.DBUser.UserId).Select(x => x.UserPlanetId).ToArray();

            // Her bir gezegenin Verify işlemini yapıyoruz.
            foreach (int userPlanetId in userPlanetIds)
                VerifyController.VerifyPlanetResources(base.UnitOfWork, new VerifyResourceDTO { UserPlanetID = userPlanetId });

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
            List<UserPlanetBuildingDTO> userPlanetsBuildings = base.UnitOfWork.GetRepository<TblUserPlanetBuildings>().Where(x => x.UserId == base.DBUser.UserId)
                .Select(x => new UserPlanetBuildingDTO
                {
                    BuildingId = x.BuildingId,
                    BuildingLevel = x.BuildingLevel,
                    UserPlanetId = x.UserPlanetId
                }).ToList();

            // Kullanıcıya ait bütün yükseltmeler.
            List<UserPlanetBuildingUpgDTO> userPlanetBuildingUpgs = base.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Where(x => x.UserId == base.DBUser.UserId)
                .Select(x => new UserPlanetBuildingUpgDTO
                {
                    BuildingId = x.BuildingId,
                    BuildingLevel = x.BuildingLevel,
                    LeftTime = (x.EndDate - base.RequestDate).TotalSeconds,
                    UserPlanetId = x.UserPlanetId
                })
                .ToList();

            // Kullanıcının araştırmaları.
            List<UserResearchesDTO> userResearches = base.UnitOfWork.GetRepository<TblUserResearches>().Where(x => x.UserId == base.DBUser.UserId).Select(x => new UserResearchesDTO
            {
                ResearchID = x.ResearchId,
                ResearchLevel = x.ResearchLevel
            }).ToList();

            // Devam eden araştırmaları alıyoruz.
            List<UserResearchProgDTO> userResearchProgs = base.UnitOfWork.GetRepository<TblUserResearchUpgs>().Where(x => x.UserId == base.DBUser.UserId).Select(x => new UserResearchProgDTO
            {
                LeftTime = (x.EndDate - base.RequestDate).TotalSeconds,
                ResearchID = x.ResearchId,
                ResearchLevel = x.ResearchTargetLevel,
            }).ToList();

            // Kullanıcının gezegenlerini buluyoruz.
            List<UserPlanetDTO> userPlanets = this.UnitOfWork.GetRepository<TblUserPlanets>().Where(x => x.UserId == base.DBUser.UserId).Select(x => new UserPlanetDTO
            {
                UserId = x.UserId,
                Boron = x.Boron,
                Crystal = x.Crystal,
                Metal = x.Metal,
                PlanetCordinate = x.PlanetCordinate,
                PlanetName = x.PlanetName,
                PlanetType = x.PlanetType,
                UserPlanetId = x.UserPlanetId
            }).ToList();

            // Kullanıcının gemilerini buluyoruz.
            List<UserPlanetShipDTO> userPlanetShips = this.UnitOfWork.GetRepository<TblUserPlanetShips>()
                .Where(x => x.UserId == base.DBUser.UserId)
                .Select(x => new UserPlanetShipDTO
                {
                    ShipCount = x.ShipCount,
                    ShipId = x.ShipId,
                    UserPlanetId = x.UserPlanetId
                }).ToList();

            // Kullanıcının gemi üretimlerini buluyoruz.
            List<UserPlanetShipProgDTO> userPlanetShipProgs = this.UnitOfWork.GetRepository<TblUserPlanetShipProgs>()
                .Where(x => x.UserId == base.DBUser.UserId)
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
            List<UserPlanetDefenseDTO> userPlanetDefenses = this.UnitOfWork.GetRepository<TblUserPlanetDefenses>()
                .Where(x => x.UserId == base.DBUser.UserId)
                .Select(x => new UserPlanetDefenseDTO
                {
                    DefenseCount = x.DefenseCount,
                    DefenseId = x.DefenseId,
                    UserPlanetId = x.UserPlanetId
                }).ToList();

            // Kullanıcının savunma üretimlerini buluyoruz.
            List<UserPlanetDefenseProgDTO> userPlanetDefenseProgs = this.UnitOfWork.GetRepository<TblUserPlanetDefenseProgs>()
                .Where(x => x.UserId == base.DBUser.UserId)
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
                UserPlanetDefenses = userPlanetDefenses
            });
        }

        [HttpPost("VerifyUserData")]
        [Description("Kullanıcının bütün datalarını doğrular.")]
        public ApiResult VerifyUserData(VerifyResourceDTO verify)
        {
            // Her bir gezegenin Verify işlemini yapıyoruz.
            VerifyController.VerifyPlanetResources(base.UnitOfWork, new VerifyResourceDTO { UserPlanetID = verify.UserPlanetID });

            // Kullanıcının gezegenlerini buluyoruz.
            UserPlanetDTO userPlanet = this.UnitOfWork.GetRepository<TblUserPlanets>().Where(x => x.UserId == base.DBUser.UserId)
                .Select(x => new UserPlanetDTO
                {
                    UserId = x.UserId,
                    Boron = x.Boron,
                    Crystal = x.Crystal,
                    Metal = x.Metal,
                    PlanetCordinate = x.PlanetCordinate,
                    PlanetName = x.PlanetName,
                    PlanetType = x.PlanetType,
                    UserPlanetId = x.UserPlanetId
                }).FirstOrDefault(x => x.UserPlanetId == verify.UserPlanetID);


            // Eğer tamamlandıysa geri dön.
            return ResponseHelper.GetSuccess(userPlanet);
        }

    }
}