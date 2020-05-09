using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;

namespace WarOfGalaxiesApi.Controllers
{
    public class BuildingController : MainController
    {
        public BuildingController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpPost("UpgradeUserPlanetBuilding")]
        [Description("Gezegendeki bina yükseltmesini yapar.")]
        public ApiResult UpgradeUserPlanetBuilding([FromForm]UserPlanetUpgradeBuildingDTO request)
        {
            // Verify İşlemini gerçekleştiriyoruz.
            bool isVerifySucceed = VerifyController.VerifyPlanetResources(base.UnitOfWork, new VerifyResourceDTO { UserPlanetID = request.UserPlanetID });

            // Eğer doğrulama başarısız ise geri dön.
            if (!isVerifySucceed)
                return ResponseHelper.GetError("Kaynaklar doğrulanırken bir hata oluştu!");

            // İlk iş gezegeni buluyoruz.
            TblUserPlanets userPlanet = base.UnitOfWork.GetRepository<TblUserPlanets>().FirstOrDefault(x => x.UserPlanetId == request.UserPlanetID && x.UserId == base.DBUser.UserId);

            // Eğer kullanıcıya ait bir gezegen yok ise geri dön.
            if (userPlanet == null)
                return ResponseHelper.GetError("Kullanıcıya ait gezegen bulunamadı!");

            // Yükseltme yapılıyor mu?
            bool isAlreadyInProg = base.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Any(x => x.UserPlanetId == request.UserPlanetID);

            // Zaten yükseltme de ise geri dön.
            if (isAlreadyInProg)
                return ResponseHelper.GetError("Zaten bir yükseltme yapılıyor.");

            // Gezegenin sahip olduğu binayı buluyoruz.
            TblUserPlanetBuildings userPlanetBuilding = base.UnitOfWork.GetRepository<TblUserPlanetBuildings>().FirstOrDefault(x => x.UserPlanetId == request.UserPlanetID && x.BuildingId == request.BuildingID);

            // Sonraki seviye.
            int nextLevel = userPlanetBuilding == null ? 1 : userPlanetBuilding.BuildingLevel + 1;

            // Yükseltme bilgisini tutuyoruz.
            ResourcesDTO upgradeInfo = StaticData.CalculateCostBuilding((Buildings)request.BuildingID, nextLevel);

            // Kaynak yeterli mi?
            if (userPlanet.Metal < upgradeInfo.Metal || userPlanet.Crystal < upgradeInfo.Crystal || userPlanet.Boron < upgradeInfo.Boron)
                return ResponseHelper.GetError("Yetersiz kaynak.");

            // Gereksinimi düşüyoruz.
            userPlanet.Metal -= upgradeInfo.Metal;
            userPlanet.Crystal -= upgradeInfo.Crystal;
            userPlanet.Boron -= upgradeInfo.Boron;

            // Robot fabrikasını buluyoruz.
            TblUserPlanetBuildings robotFactory = base.UnitOfWork.GetRepository<TblUserPlanetBuildings>().FirstOrDefault(x => x.UserPlanetId == request.UserPlanetID && x.BuildingId == (int)Buildings.RobotFabrikası);

            // Yükseltme süresi.
            double upgradeTime = StaticData.CalculateBuildingUpgradeTime((Buildings)request.BuildingID, nextLevel, robotFactory == null ? 0 : robotFactory.BuildingLevel);

            // Bitiş tarihi.
            DateTime endDate = base.RequestDate.AddSeconds(upgradeTime);

            // Yükseltmesini yapıyoruz.
            TblUserPlanetBuildingUpgs userPlanetUpg = base.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Add(new TblUserPlanetBuildingUpgs
            {
                UserPlanetId = request.UserPlanetID,
                BeginDate = base.RequestDate,
                BuildingId = request.BuildingID,
                BuildingLevel = nextLevel,
                EndDate = endDate,
                UserId = base.DBUser.UserId
            });

            // Değişiklikleri kayıt ediyoruz.
            base.UnitOfWork.SaveChanges();

            // En son başarılı sonucunu dönüyoruz.
            return ResponseHelper.GetSuccess(new UserPlanetBuildingUpgDTO
            {
                BuildingId = userPlanetUpg.BuildingId,
                BuildingLevel = userPlanetUpg.BuildingLevel,
                LeftTime = (userPlanetUpg.EndDate - userPlanetUpg.BeginDate).TotalSeconds,
                UserPlanetId = userPlanetUpg.UserPlanetId,
                PlanetResources = new ResourcesDTO(userPlanet.Metal, userPlanet.Crystal, userPlanet.Boron)
            });
        }

    }
}