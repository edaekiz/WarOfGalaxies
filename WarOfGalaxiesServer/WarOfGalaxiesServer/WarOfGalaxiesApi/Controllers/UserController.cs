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
                    LeftTime = (x.EndDate - x.BeginDate).TotalSeconds,
                    UserPlanetId = x.UserPlanetId
                })
                .ToList();

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

            // Sonucu dönüyoruz.
            return ResponseHelper.GetSuccess(new LoginStuffDTO
            {
                UserData = user,
                UserPlanetsBuildings = userPlanetsBuildings,
                UserPlanetsBuildingsUpgs = userPlanetBuildingUpgs,
                UserPlanets = userPlanets
            });
        }

    }
}