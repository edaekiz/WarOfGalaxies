using Microsoft.AspNetCore.Mvc;
using System;
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
    public class BuildingController : MainController
    {
        public BuildingController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpPost("GetBuildingLevels")]
        [Description("Sisteme ait bütün binaların yükseltme ve üretim bilgileri.")]
        public ApiResult GetBuildingLevels()
        {
            // Bina seviyeleri.
            List<BuildingLevelDTO> buildingLevels = base.UnitOfWork.GetRepository<TblBuildingLevels>().All().Select(x => new BuildingLevelDTO
            {
                BuildingId = x.BuildingId,
                BuildingLevel = x.BuildingLevel,
                BuildingValue = x.BuildingValue,
                RequiredBoron = x.RequiredBoron,
                RequiredCrystal = x.RequiredCrystal,
                RequiredEnergy = x.RequiredEnergy,
                RequiredMetal = x.RequiredMetal,
                UpgradeTime = x.UpgradeTime
            }).ToList();

            // Binaları seviyelerini geri döner.
            return ResponseHelper.GetSuccess(buildingLevels);
        }

        [HttpPost("GetUserBuildings")]
        [Description("Kullanıcıya ait bütün gezgenlerdeki binalar.")]
        public ApiResult GetUserBuildings()
        {
            // Kullanıcının gezegenlerine ait binalar.
            List<UserPlanetBuildingDTO> userPlanesBuildings = base.UnitOfWork.GetRepository<TblUserPlanetBuildings>().Where(x => x.UserId == base.DBUser.UserId)
                .Select(x => new UserPlanetBuildingDTO
                {
                    BuildingId = x.BuildingId,
                    BuildingLevel = x.BuildingLevel,
                    UserPlanetId = x.UserPlanetId
                }).ToList();

            // Kayıtları dönüyoruz.
            return ResponseHelper.GetSuccess(userPlanesBuildings);
        }

        [HttpPost("GetUserBuildingsProgs")]
        [Description("Kullanıcya ait bütün gezegenlerdeki devam eden yükseltmeler.")]
        public ApiResult GetUserBuildingsProgs()
        {
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

            // Geri dönüyoruz.
            return ResponseHelper.GetSuccess(userPlanetBuildingUpgs);
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
            TblBuildingLevels upgradeInfo = null;

            // Eğer yükseltme yok ise demek oluyor ki henüz inşaa edilecek.
            if (userPlanetBuilding == null)
                upgradeInfo = base.UnitOfWork.GetRepository<TblBuildingLevels>().FirstOrDefault(x => x.BuildingId == request.BuildingID && x.BuildingLevel == 1);
            else // Aksi durumda yükseltme yapılacak.
                upgradeInfo = base.UnitOfWork.GetRepository<TblBuildingLevels>().FirstOrDefault(x => x.BuildingId == request.BuildingID && x.BuildingLevel == userPlanetBuilding.BuildingLevel + 1);

            // Eğer yükseltme bilgisi yok ise hata dön.
            if (upgradeInfo == null)
                return ResponseHelper.GetError("Yükseltme bilgisi bulunamadı!");

            // Gereksinimi düşüyoruz.
            userPlanet.Metal -= upgradeInfo.RequiredMetal;
            userPlanet.Crystal -= upgradeInfo.RequiredCrystal;
            userPlanet.Boron -= upgradeInfo.RequiredBoron;

            // Yükseltmesini yapıyoruz.
            TblUserPlanetBuildingUpgs userPlanetUpg = base.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Add(new TblUserPlanetBuildingUpgs
            {
                UserPlanetId = request.UserPlanetID,
                BeginDate = currentDate,
                BuildingId = request.BuildingID,
                BuildingLevel = upgradeInfo.BuildingLevel,
                EndDate = currentDate.AddSeconds(upgradeInfo.UpgradeTime),
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