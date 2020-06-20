using Microsoft.AspNetCore.Mvc;
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
    public class ShipyardController : MainController
    {
        public ShipyardController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
        {
        }

        [HttpPost("AddShipToShipyardQueue")]
        [Description("Kullanıcının tersaneye gemi üretim görevi eklemesini sağlar.")]
        public ApiResult AddShipToShipyardQueue(ShipyardAddQueueRequestDTO request)
        {
            // Kaynakları doğruluyoruz.
            TblUserPlanets userPlanet = VerifyController.VerifyAllFleets(this, new VerifyResourceDTO { UserPlanetID = request.UserPlanetID });

            // Eğer gezegen yok ise hata dön.
            if (userPlanet == null || userPlanet.UserId != base.DBUser.UserId)
                return ResponseHelper.GetError("Kullanıcıya ait gezegen bulunamadı!");

            // Gemi bilgisini buluyoruz.
            TblShips shipInfo = StaticValues.GetShip(request.ShipID);

            // Gemi yok ise hata dön.
            if (shipInfo == null)
                return ResponseHelper.GetError("Gemi bulunamadı!");

            // Bu teknoloji keşfedildi mi?
            bool isInvented = StaticValues.IsTechInvented(userPlanet, TechnologyCategories.Gemiler, shipInfo.ShipId);

            // Teknoloji henüz keşfedilmedi uyarısı dönüyoruz.
            if (!isInvented)
                return ResponseHelper.GetError("Teknoloji henüz keşfedilmedi!");

            // Gemi maliyeti.
            ResourcesDTO shipCost = new ResourcesDTO(shipInfo.CostMetal, shipInfo.CostCrystal, shipInfo.CostBoron);

            // Üretilmek istenen miktar ile gereksinimi çarpıyoruz. genel toplamı buluyoruz.
            ResourcesDTO totalCalculatedRes = shipCost * request.Quantity;

            // Eğer gezegende tersane yok ise geri dön.
            if (!base.UnitOfWork.GetRepository<TblUserPlanetBuildings>().Any(x => x.UserPlanetId == userPlanet.UserPlanetId && x.BuildingId == (int)Buildings.Tersane))
                return ResponseHelper.GetError("Bu gezegende tersane bulunmuyor!");

            // Eğer gezegen de yükseltme var ise dönüyoruz.
            if (base.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Any(x => x.UserPlanetId == userPlanet.UserPlanetId && x.BuildingId == (int)Buildings.Tersane))
                return ResponseHelper.GetError("Bu gezegende tersane yükseltiliyor!");

            // Gezegendeki kaynakları dönüştürüyoruz.
            ResourcesDTO planetRes = new ResourcesDTO(userPlanet.Metal, userPlanet.Crystal, userPlanet.Boron);

            // Hata almamak için 0 dan büyük olması koşulunu ekliyoruz.
            if (shipCost.Metal > 0)
            {
                // Gezegendeki kaynakların toplamı ile kaç adet üretilebilir.
                int maxMetalQuantity = (int)(planetRes.Metal / shipCost.Metal);

                // Eğer gezegendeki toplam üretilebilir miktar kullanıcının yazdığından az ise üretim miktarını azaltıyoruz.
                if (maxMetalQuantity < request.Quantity)
                    request.Quantity = maxMetalQuantity;
            }

            // Hata almamak için 0 dan büyük olması koşulunu ekliyoruz.
            if (shipCost.Crystal > 0)
            {
                // Gezegendeki kaynakların toplamı ile kaç adet üretilebilir.
                int maxCrystalQuantity = (int)(planetRes.Crystal / shipCost.Crystal);

                // Eğer gezegendeki toplam üretilebilir miktar kullanıcının yazdığından az ise üretim miktarını azaltıyoruz.
                if (maxCrystalQuantity < request.Quantity)
                    request.Quantity = maxCrystalQuantity;
            }

            // Hata almamak için 0 dan büyük olması koşulunu ekliyoruz.
            if (shipCost.Boron > 0)
            {
                // Gezegendeki kaynakların toplamı ile kaç adet üretilebilir.
                int maxBoronQuantity = (int)(planetRes.Boron / shipCost.Boron);

                // Eğer gezegendeki toplam üretilebilir miktar kullanıcının yazdığından az ise üretim miktarını azaltıyoruz.
                if (maxBoronQuantity < request.Quantity)
                    request.Quantity = maxBoronQuantity;
            }

            // Miktar 0 ise geri dön.
            if (request.Quantity <= 0)
                return ResponseHelper.GetError("Gemi miktarı 0 olamaz");

            // Düşülecek miktar.
            ResourcesDTO calculatedCost = shipCost * request.Quantity;

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
