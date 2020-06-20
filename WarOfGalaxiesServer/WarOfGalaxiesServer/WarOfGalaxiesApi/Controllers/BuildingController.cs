using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.ComponentModel;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public class BuildingController : MainController
    {
        public BuildingController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
        {
        }

        [HttpPost("UpgradeUserPlanetBuilding")]
        [Description("Gezegendeki bina yükseltmesini yapar.")]
        public ApiResult UpgradeUserPlanetBuilding([FromForm] UserPlanetUpgradeBuildingDTO request)
        {
            // Verify İşlemini gerçekleştiriyoruz.
            TblUserPlanets userPlanet = VerifyController.VerifyAllFleets(this, new VerifyResourceDTO { UserPlanetID = request.UserPlanetID });

            // Eğer kullanıcıya ait bir gezegen yok ise geri dön.
            if (userPlanet == null || userPlanet.UserId != base.DBUser.UserId)
                return ResponseHelper.GetError("Kullanıcıya ait gezegen bulunamadı!");

            // Bu teknoloji keşfedildi mi?
            bool isInvented = StaticValues.IsTechInvented(userPlanet, TechnologyCategories.Binalar, request.BuildingID);

            // Teknoloji henüz keşfedilmedi uyarısı dönüyoruz.
            if (!isInvented)
                return ResponseHelper.GetError("Teknoloji henüz keşfedilmedi!");

            // Yükseltme yapılıyor mu?
            bool isAlreadyInProg = base.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Any(x => x.UserPlanetId == request.UserPlanetID);

            // Zaten yükseltme de ise geri dön.
            if (isAlreadyInProg)
                return ResponseHelper.GetError("Zaten bir yükseltme yapılıyor.");

            #region Eğer gemi üretimi yapıyor isek tersane üretemeyiz.

            // Tersaneyi yükseltirken kontrol etmeliyiz, gemi üretimi var mı?
            if (request.BuildingID == (int)Buildings.Tersane)
            {
                // Gezegende gemi üretimi yapıyor muyuz?
                bool isThereShipProduce = base.UnitOfWork.GetRepository<TblUserPlanetShipProgs>().Any(x => x.UserPlanetId == request.UserPlanetID);

                // Eğer yapıyor isek hata döneceğiz.
                if (isThereShipProduce)
                    return ResponseHelper.GetError("Bu gezgende gemi üretimi mevcut!");
            }

            #endregion

            #region Eğer araştırma yapıyorsak labratuvar yükseltilemez.

            // Araştırma yükseltirken kontrol etmeliyiz, yükseltme var mı?
            if (request.BuildingID == (int)Buildings.ArastirmaLab)
            {
                // Gezegende araştırma yapıyor muyuz?
                bool isThereResearchUpg = base.UnitOfWork.GetRepository<TblUserResearchUpgs>().Any(x => x.UserId == DBUser.UserId);

                // Eğer yapıyor isek hata döneceğiz.
                if (isThereResearchUpg)
                    return ResponseHelper.GetError("Bu gezgende araştırma yükseltmesi mevcut!");
            }

            #endregion

            #region Eğer savunma birimi üretiyorsak robot fabrikası yükseltilemez.

            // Robot fabrikasını yükseltirken kontrol ediyoruz savunma sistemi üretliyor mu?
            if (request.BuildingID == (int)Buildings.RobotFabrikası)
            {
                // Gezegende savunma üretimi yapıyor muyuz?
                bool isThereDefenseProg = base.UnitOfWork.GetRepository<TblUserPlanetDefenseProgs>().Any(x => x.UserPlanetId == request.UserPlanetID);

                // Eğer yapıyor isek hata döneceğiz.
                if (isThereDefenseProg)
                    return ResponseHelper.GetError("Bu gezgende savunma üretimi mevcut!");
            }

            #endregion

            // Gezegenin sahip olduğu binayı buluyoruz.
            TblUserPlanetBuildings userPlanetBuilding = base.UnitOfWork.GetRepository<TblUserPlanetBuildings>().FirstOrDefault(x => x.UserPlanetId == request.UserPlanetID && x.BuildingId == request.BuildingID);

            // Sonraki seviye.
            int nextLevel = userPlanetBuilding == null ? 1 : userPlanetBuilding.BuildingLevel + 1;

            // Yükseltme bilgisini tutuyoruz.
            ResourcesDTO upgradeInfo = StaticValues.CalculateCostBuilding((Buildings)request.BuildingID, nextLevel);

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
            double upgradeTime = StaticValues.CalculateBuildingUpgradeTime((Buildings)request.BuildingID, nextLevel, robotFactory == null ? 0 : robotFactory.BuildingLevel);

            // Bitiş tarihi.
            DateTime endDate = base.RequestDate.AddSeconds(upgradeTime);

            // Yükseltmesini yapıyoruz.
            TblUserPlanetBuildingUpgs userPlanetUpg = base.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Add(new TblUserPlanetBuildingUpgs
            {
                UserPlanetId = request.UserPlanetID,
                BeginDate = base.RequestDate,
                BuildingId = request.BuildingID,
                BuildingLevel = nextLevel,
                EndDate = endDate
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