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
        public ApiResult UpgradeUserPlanetBuilding(UserPlanetUpgradeBuildingDTO request)
        {
            // Sistem anlık tarihi.
            DateTime currentDate = DateTime.UtcNow;

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

            // Yükseltme bilgisini tutuyoruz.
            ResourcesDTO upgradeInfo = StaticData.CalculateCostBuilding((Buildings)request.BuildingID, userPlanetBuilding == null ? 1 : userPlanetBuilding.BuildingLevel);

            // Kaynak yeterli mi?
            if (userPlanet.Metal < upgradeInfo.Metal || userPlanet.Crystal < upgradeInfo.Crystal || userPlanet.Boron < upgradeInfo.Boron)
                return ResponseHelper.GetError("Yetersiz kaynak.");

            // Gereksinimi düşüyoruz.
            userPlanet.Metal -= upgradeInfo.Metal;
            userPlanet.Crystal -= upgradeInfo.Metal;
            userPlanet.Boron -= upgradeInfo.Metal;

            // Yükseltme süresi.
            double upgradeTime = StaticData.CalculateBuildingUpgradeTime((Buildings)request.BuildingID, 0);

            // Yükseltmesini yapıyoruz.
            TblUserPlanetBuildingUpgs userPlanetUpg = base.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Add(new TblUserPlanetBuildingUpgs
            {
                UserPlanetId = request.UserPlanetID,
                BeginDate = currentDate,
                BuildingId = request.BuildingID,
                BuildingLevel = userPlanetBuilding == null ? 1 : userPlanetBuilding.BuildingLevel,
                EndDate = currentDate.AddSeconds(upgradeTime),
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
                UserPlanetId = userPlanetUpg.UserPlanetId
            });
        }

    }
}