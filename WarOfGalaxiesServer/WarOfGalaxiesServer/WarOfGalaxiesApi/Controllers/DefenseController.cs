using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;

namespace WarOfGalaxiesApi.Controllers
{
    public class DefenseController : MainController
    {
        public DefenseController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpPost("AddDefenseToDefenseQueue")]
        [Description("Kullanıcının robot fabrikasına savunma üretim görevi eklemesini sağlar.")]
        public ApiResult AddDefenseToDefenseQueue(DefenseAddQueueRequestDTO request)
        {
            // Kaynakları doğruluyoruz.
            bool isVerified = VerifyController.VerifyPlanetResources(base.UnitOfWork, new VerifyResourceDTO { UserPlanetID = request.UserPlanetID });

            // Eğer doğrulanamaz ise hata dön.
            if (!isVerified)
                return ResponseHelper.GetError("Kaynaklar doğrulanırken hata oluştu");

            // Savunma bilgisini buluyoruz.
            DefenseDTO defenseInfo = StaticData.DefenseData.Find(x => x.DefenseID == request.DefenseID);

            // Savunma yok ise hata dön.
            if (defenseInfo == null)
                return ResponseHelper.GetError("Savunma bulunamadı!");

            // Üretilmek istenen miktar ile gereksinimi çarpıyoruz. genel toplamı buluyoruz.
            ResourcesDTO totalCalculatedRes = defenseInfo.Cost * request.Quantity;

            // Gezegenin kaynaklarını alıyoruz.
            TblUserPlanets userPlanet = base.UnitOfWork.GetRepository<TblUserPlanets>().FirstOrDefault(x => x.UserPlanetId == request.UserPlanetID && x.UserId == base.DBUser.UserId);

            // Eğer gezegen yok ise hata dön.
            if (userPlanet == null)
                return ResponseHelper.GetError("Kullanıcıya ait gezegen bulunamadı!");

            // Gezegendeki kaynakları dönüştürüyoruz.
            ResourcesDTO planetRes = new ResourcesDTO(userPlanet.Metal, userPlanet.Crystal, userPlanet.Boron);

            // Hata almamak için 0 dan büyük olması koşulunu ekliyoruz.
            if (defenseInfo.Cost.Metal > 0)
            {
                // Gezegendeki kaynakların toplamı ile kaç adet üretilebilir.
                int maxMetalQuantity = (int)(planetRes.Metal / defenseInfo.Cost.Metal);

                // Eğer gezegendeki toplam üretilebilir miktar kullanıcının yazdığından az ise üretim miktarını azaltıyoruz.
                if (maxMetalQuantity < request.Quantity)
                    request.Quantity = maxMetalQuantity;
            }

            // Hata almamak için 0 dan büyük olması koşulunu ekliyoruz.
            if (defenseInfo.Cost.Crystal > 0)
            {
                // Gezegendeki kaynakların toplamı ile kaç adet üretilebilir.
                int maxCrystalQuantity = (int)(planetRes.Crystal / defenseInfo.Cost.Crystal);

                // Eğer gezegendeki toplam üretilebilir miktar kullanıcının yazdığından az ise üretim miktarını azaltıyoruz.
                if (maxCrystalQuantity < request.Quantity)
                    request.Quantity = maxCrystalQuantity;
            }

            // Hata almamak için 0 dan büyük olması koşulunu ekliyoruz.
            if (defenseInfo.Cost.Boron > 0)
            {
                // Gezegendeki kaynakların toplamı ile kaç adet üretilebilir.
                int maxBoronQuantity = (int)(planetRes.Boron / defenseInfo.Cost.Boron);

                // Eğer gezegendeki toplam üretilebilir miktar kullanıcının yazdığından az ise üretim miktarını azaltıyoruz.
                if (maxBoronQuantity < request.Quantity)
                    request.Quantity = maxBoronQuantity;
            }

            // Miktar 0 ise geri dön.
            if (request.Quantity <= 0)
                return ResponseHelper.GetError("Savunma miktarı 0 olamaz");

            // Düşülecek miktar.
            ResourcesDTO calculatedCost = defenseInfo.Cost * request.Quantity;

            // Kaynakları düşüyoruz.
            userPlanet.Metal -= calculatedCost.Metal;
            userPlanet.Crystal -= calculatedCost.Crystal;
            userPlanet.Boron -= calculatedCost.Boron;

            // Bu gezegende başka bir üretim var mı?
            bool isThereQueueInShipyard = base.UnitOfWork.GetRepository<TblUserPlanetDefenseProgs>().Any(x => x.UserPlanetId == request.UserPlanetID);

            // Üretim sırasına eklemek.
            base.UnitOfWork.GetRepository<TblUserPlanetDefenseProgs>().Add(new TblUserPlanetDefenseProgs
            {
                LastVerifyDate = isThereQueueInShipyard ? null : (DateTime?)base.RequestDate,
                DefenseCount = request.Quantity,
                DefenseId = (int)request.DefenseID,
                UserId = base.DBUser.UserId,
                UserPlanetId = request.UserPlanetID
            });

            // Değişiklikleri kayıt ediyoruz.
            base.UnitOfWork.SaveChanges();

            // Sonucu dönüyoruz.
            return ResponseHelper.GetSuccess(new DefenseAddQueueResponseDTO
            {
                PlanetResources = new ResourcesDTO(userPlanet.Metal, userPlanet.Crystal, userPlanet.Boron),
                Quantity = request.Quantity,
                DefenseID = (int)request.DefenseID,
                UserPlanetID = request.UserPlanetID
            });
        }

    }
}
