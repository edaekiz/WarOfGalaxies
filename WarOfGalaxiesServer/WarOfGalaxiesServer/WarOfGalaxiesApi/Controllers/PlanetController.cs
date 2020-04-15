using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;

namespace WarOfGalaxiesApi.Controllers
{
    public class PlanetController : MainController
    {
        public PlanetController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        [HttpPost("GetUserPlanets")]
        [Description("Kullanıcının gezegenlerini buluyoruz.")]
        public ApiResult GetUserPlanets()
        {
            // Kullanıcının gezegenlerinin idsi.
            int[] userPlanetIds = this.UnitOfWork.GetRepository<TblUserPlanets>().Where(x => x.UserId == base.DBUser.UserId).Select(x => x.UserPlanetId).ToArray();

            // Her bir gezegenin Verify işlemini yapıyoruz.
            foreach (int userPlanetId in userPlanetIds)
                VerifyController.VerifyPlanetResources(base.UnitOfWork, new VerifyResourceDTO { UserPlanetID = userPlanetId });

            // Kullanıcının gezegenlerini buluyoruz.
            UserPlanetDTO[] userPlanets = this.UnitOfWork.GetRepository<TblUserPlanets>().Where(x => x.UserId == base.DBUser.UserId).Select(x => new UserPlanetDTO
            {
                UserId = x.UserId,
                Boron = x.Boron,
                Crystal = x.Crystal,
                LastUpdateDate = x.LastUpdateDate,
                Metal = x.Metal,
                PlanetCordinate = x.PlanetCordinate,
                PlanetName = x.PlanetName,
                PlanetType = x.PlanetType,
                UserPlanetId = x.UserPlanetId
            }).ToArray();

            // Kullanıcının gezegenlerini dönüyoruz.
            return ResponseHelper.GetSuccess(userPlanets);
        }
    }
}