using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public class ResearchController : MainController
    {
        public ResearchController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
        {
        }

        [HttpPost("UpgradeUserResearch")]
        [Description("Kullanıcının verilen araştırmasını yükseltir.")]
        public ApiResult UpgradeUserResearch(UserResearchUpgRequest request)
        {
            // Yapılacak ilk iş kaynakları doğrulamak.
            VerifyController.VerifyAllFleets(this, new VerifyResourceDTO { UserPlanetID = request.UserPlanetID });

            // Kullanıcının hangi gezegendeki kaynakları kullanılacak.
            TblUserPlanets userPlanet = base.UnitOfWork.GetRepository<TblUserPlanets>().FirstOrDefault(x => x.UserPlanetId == request.UserPlanetID && x.UserId == base.DBUser.UserId);

            // Kullanıcının gezegeni yok ise geri dön.
            if (userPlanet == null)
                return ResponseHelper.GetError("Kullanıcının gezegeni yok ise geri dön.");

            // Zaten bir yükseltme yapılıyor mu?
            bool isAlreadyInUpg = base.UnitOfWork.GetRepository<TblUserResearchUpgs>().Any(x => x.UserId == base.DBUser.UserId);

            // Zaten bir yükseltme yapılıyor ise geri dön.
            if (isAlreadyInUpg)
                return ResponseHelper.GetError("Zaten bir yükseltme yapılıyor.");

            // Kullanıcının araştırmasını alıyoruz.
            TblUserResearches userExistsResearch = base.UnitOfWork.GetRepository<TblUserResearches>().FirstOrDefault(x => x.UserId == base.DBUser.UserId && x.ResearchId == (int)request.ResearchID);

            int nextResearchLevel = userExistsResearch == null ? 1 : userExistsResearch.ResearchLevel + 1;

            // Kullanıcı daha önce araştırmış ise araştırma seviyesini araştırmamış ise 1.seviyeye göre kaynakları hesaplıyoruz.
            ResourcesDTO researchCost = StaticValues.CalculateCostResearch(request.ResearchID, nextResearchLevel);

            // Eğer kullanıcının gezegeninde yeterli kaynak yok ise geri dön.
            if (userPlanet.Metal < researchCost.Metal || userPlanet.Crystal < researchCost.Crystal || userPlanet.Boron < researchCost.Boron)
                return ResponseHelper.GetError("Yetersiz hammadde.");

            // Yükseltme süresi
            double researchUpgTime = StaticValues.CalculateResearchUpgradeTime(request.ResearchID, nextResearchLevel);

            // Eğer var ise yükseltmeyi başlatabiliriz.
            TblUserResearchUpgs upgradeData = base.UnitOfWork.GetRepository<TblUserResearchUpgs>().Add(new TblUserResearchUpgs
            {
                BeginDate = base.RequestDate,
                EndDate = base.RequestDate.AddSeconds(researchUpgTime),
                ResearchId = (int)request.ResearchID,
                ResearchTargetLevel = nextResearchLevel,
                UserId = base.DBUser.UserId,
                UserPlanetId = request.UserPlanetID
            });

            // Kullanıcıdan kaynakları düşüyoruz.
            userPlanet.Metal -= researchCost.Metal;
            userPlanet.Crystal -= researchCost.Crystal;
            userPlanet.Boron -= researchCost.Boron;

            // Değişiklikleri kayıt ediyoruz.
            base.UnitOfWork.SaveChanges();

            // Başarılı sonucunu dönüyoruz.
            return ResponseHelper.GetSuccess(new UserResearchProgDTO
            {
                LeftTime = (upgradeData.EndDate - upgradeData.BeginDate).TotalSeconds,
                ResearchID = (int)request.ResearchID,
                ResearchLevel = nextResearchLevel,
                UserPlanetID = upgradeData.UserPlanetId,
                Resources = new ResourcesDTO(userPlanet.Metal, userPlanet.Crystal, userPlanet.Boron)
            });
        }


    }
}
