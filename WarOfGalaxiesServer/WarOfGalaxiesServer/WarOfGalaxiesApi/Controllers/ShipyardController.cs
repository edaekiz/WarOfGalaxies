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
    public class ShipyardController : MainController
    {
        public ShipyardController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpPost("AddShipToShipyardQueue")]
        [Description("Kullanıcının tersaneye gemi üretim görevi eklemesini sağlar.")]
        public ApiResult AddShipToShipyardQueue(ShipyardAddQueueRequestDTO request)
        {
            // Kaynakları doğruluyoruz.
            bool isVerified = VerifyController.VerifyPlanetResources(base.UnitOfWork, new VerifyResourceDTO { UserPlanetID = request.UserPlanetID });

            // Eğer doğrulanamaz ise hata dön.
            if (!isVerified)
                return ResponseHelper.GetError("Kaynaklar doğrulanırken hata oluştu");

            // Gemi bilgisini buluyoruz.
            ShipDTO shipInfo = StaticData.ShipData.Find(x => x.ShipID == request.ShipID);

            // Gemi yok ise hata dön.
            if (shipInfo == null)
                return ResponseHelper.GetError("Gemi bulunamadı!");

            // Üretilmek istenen miktar ile gereksinimi çarpıyoruz. genel toplamı buluyoruz.
            ResourcesDTO totalCalculatedRes = shipInfo.Cost * request.Quantity;

            // Gezegenin kaynaklarını alıyoruz.
            TblUserPlanets userPlanet = base.UnitOfWork.GetRepository<TblUserPlanets>().FirstOrDefault(x => x.UserPlanetId == request.UserPlanetID && x.UserId == base.DBUser.UserId);

            // Eğer gezegen yok ise hata dön.
            if (userPlanet == null)
                return ResponseHelper.GetError("Kullanıcıya ait gezegen bulunamadı!");

            // Gezegendeki kaynakları dönüştürüyoruz.
            ResourcesDTO planetRes = new ResourcesDTO(userPlanet.Metal, userPlanet.Crystal, userPlanet.Boron);

            // Hata almamak için 0 dan büyük olması koşulunu ekliyoruz.
            if (shipInfo.Cost.Metal > 0)
            {
                // Gezegendeki kaynakların toplamı ile kaç adet üretilebilir.
                int maxMetalQuantity = (int)(planetRes.Metal / shipInfo.Cost.Metal);

                // Eğer gezegendeki toplam üretilebilir miktar kullanıcının yazdığından az ise üretim miktarını azaltıyoruz.
                if (maxMetalQuantity < request.Quantity)
                    request.Quantity = maxMetalQuantity;
            }

            // Hata almamak için 0 dan büyük olması koşulunu ekliyoruz.
            if (shipInfo.Cost.Crystal > 0)
            {
                // Gezegendeki kaynakların toplamı ile kaç adet üretilebilir.
                int maxCrystalQuantity = (int)(planetRes.Crystal / shipInfo.Cost.Crystal);

                // Eğer gezegendeki toplam üretilebilir miktar kullanıcının yazdığından az ise üretim miktarını azaltıyoruz.
                if (maxCrystalQuantity < request.Quantity)
                    request.Quantity = maxCrystalQuantity;
            }

            // Hata almamak için 0 dan büyük olması koşulunu ekliyoruz.
            if (shipInfo.Cost.Boron > 0)
            {
                // Gezegendeki kaynakların toplamı ile kaç adet üretilebilir.
                int maxBoronQuantity = (int)(planetRes.Boron / shipInfo.Cost.Boron);

                // Eğer gezegendeki toplam üretilebilir miktar kullanıcının yazdığından az ise üretim miktarını azaltıyoruz.
                if (maxBoronQuantity < request.Quantity)
                    request.Quantity = maxBoronQuantity;
            }

            // Miktar 0 ise geri dön.
            if (request.Quantity <= 0)
                return ResponseHelper.GetError("Gemi miktarı 0 olamaz");

            // Düşülecek miktar.
            ResourcesDTO calculatedCost = shipInfo.Cost * request.Quantity;

            // Kaynakları düşüyoruz.
            userPlanet.Metal -= calculatedCost.Metal;
            userPlanet.Crystal -= calculatedCost.Crystal;
            userPlanet.Boron -= calculatedCost.Boron;

            // Bu gezegende başka bir üretim var mı?
            bool isThereQueueInShipyard = base.UnitOfWork.GetRepository<TblUserPlanetShipProgs>().Any(x => x.UserPlanetId == request.UserPlanetID);

            // Üretim sırasına eklemek.
            base.UnitOfWork.GetRepository<TblUserPlanetShipProgs>().Add(new TblUserPlanetShipProgs
            {
                LastVerifyDate = isThereQueueInShipyard ? null : (DateTime?)base.RequestDate,
                ShipCount = request.Quantity,
                ShipId = (int)request.ShipID,
                UserId = base.DBUser.UserId,
                UserPlanetId = request.UserPlanetID
            });

            // Değişiklikleri kayıt ediyoruz.
            base.UnitOfWork.SaveChanges();

            // Sonucu dönüyoruz.
            return ResponseHelper.GetSuccess(new ShipyardAddQueueResponseDTO
            {
                PlanetResources = new ResourcesDTO(userPlanet.Metal, userPlanet.Crystal, userPlanet.Boron),
                Quantity = request.Quantity,
                ShipID = (int)request.ShipID,
                UserPlanetID = request.UserPlanetID
            });
        }
    }
}
