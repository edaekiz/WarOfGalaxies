using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Extends;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;
using Microsoft.EntityFrameworkCore;

namespace WarOfGalaxiesApi.Controllers
{
    public class FleetController : MainController
    {
        public FleetController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpPost("GetLastFleets")]
        [Description("Son filo hareketlerini döner.")]
        public ApiResult GetLastFleets(GetLastFleetsDTO request)
        {
            // Kullanıcıya gönderilen yada kullanıcıdan gönderilen filoların listesini alıyoruz.
            List<FleetDTO> lastFleets = base.UnitOfWork.GetRepository<TblFleets>()
                .Where(x => x.FleetId > request.LastFleetId)
                .Where(x => x.SenderUserPlanet.UserId == DBUser.UserId || (x.DestinationUserPlanetId.HasValue && x.DestinationUserPlanet.UserId == DBUser.UserId))
                .Select(x => new FleetDTO
                {
                    SenderCordinate = x.SenderCordinate,
                    SenderUserId = x.SenderUserPlanet.UserId,
                    SenderUserPlanetId = x.SenderUserPlanetId,
                    SenderPlanetName = x.SenderUserPlanet.PlanetName,
                    DestinationCordinate = x.DestinationCordinate,
                    DestinationUserPlanetId = x.DestinationUserPlanetId,
                    DestinationUserId = x.DestinationUserPlanetId.HasValue ? (int?)x.DestinationUserPlanet.UserId : null,
                    DestinationPlanetName = x.DestinationUserPlanetId.HasValue ? x.DestinationUserPlanet.PlanetName : null,
                    FleetActionTypeId = x.FleetActionTypeId,
                    FleetId = x.FleetId,
                    IsReturning = x.IsReturning,
                    BeginPassedTime = (RequestDate - x.BeginDate).TotalSeconds,
                    EndLeftTime = (x.EndDate - RequestDate).TotalSeconds,
                    CarriedBoron = x.CarriedBoron,
                    CarriedCrystal = x.CarriedCrystal,
                    CarriedMetal = x.CarriedMetal
                }).ToList();

            return ResponseHelper.GetSuccess(lastFleets);
        }

        [HttpPost("FlyNewFleet")]
        [Description("Gezegenden filo çıkarma.")]
        public ApiResult FlyNewFleet(SendFleetFromPlanetDTO request)
        {
            // Eğer geçersiz bir kordinat ise geri dön.
            if (request.DestinationGalaxyIndex > 1 || request.DestinationSolarIndex > 100 || request.DestinationOrderIndex > 8)
                return ResponseHelper.GetError("Kordinat bulunamadı!");

            // Filo hızının geçerliliğini kontrol ediyoruz.
            if (request.FleetSpeed < 0 || request.FleetSpeed > 1)
                return ResponseHelper.GetError("Geçersiz filo hızı!");

            // Gelen gemilerin bilsini anlaşılır formata çeviriyoruz.
            List<Tuple<Ships, int>> shipsInData = FleetDataToShipData(request.Ships);

            // Eğer hiç gemi yok ise yola çıkmıyoruz.
            if (shipsInData.Count == 0)
                return ResponseHelper.GetError("Geçersiz veri tipi yada hiç gemi bulunamadı!");

            // Doğrulama tamamlandı mı?
            bool isVerified = VerifyController.VerifyPlanetResources(base.UnitOfWork, new VerifyResourceDTO { UserPlanetID = request.SenderUserPlanetId });

            // Eğer hata verdiyse dön.
            if (!isVerified)
                return ResponseHelper.GetError("Kaynaklar doğrulanamadı!");

            // Kullanıcının gezegeni.
            TblUserPlanets userPlanet = base.UnitOfWork.GetRepository<TblUserPlanets>().Where(x => x.UserPlanetId == request.SenderUserPlanetId && x.UserId == DBUser.UserId)
                .Include(x => x.TblCordinates).FirstOrDefault();

            // Gezegen yok ise hata dön.
            if (userPlanet == null)
                return ResponseHelper.GetError("Gezegeni bulamadık!");

            // Kullanıcının filosunu alıyoruz.
            List<TblUserPlanetShips> userPlanetShips = base.UnitOfWork.GetRepository<TblUserPlanetShips>().Where(x => x.UserPlanetId == request.SenderUserPlanetId).ToList();

            // Her bir gemiyi tersaneden düşüyoruz.
            foreach (Tuple<Ships, int> ship in shipsInData)
            {
                // Kullanıcıya ait gemiyi buluyoruz.
                TblUserPlanetShips planetShip = userPlanetShips.Find(x => x.ShipId == (int)ship.Item1);

                // Eğer gemi yok ise yada miktarı az ise hata dönüyoruz.
                if (planetShip == null || planetShip.ShipCount < ship.Item2)
                    return ResponseHelper.GetError("Yeterli gemi yoktu!");

                // Tersaneden düşüyoruz.
                planetShip.ShipCount -= ship.Item2;

                // Eğer başka gemi kalmamış ise kaydı siliyoruz.
                if (planetShip.ShipCount <= 0)
                    base.UnitOfWork.GetRepository<TblUserPlanetShips>().Delete(planetShip);
            }

            // Minimum gemi hızı.
            int minSpeed = GetMinSpeedInFleet(shipsInData.Select(x => x.Item1));

            // Kullanıcının toplam göndereceği miktar.
            double totalResource = request.CarriedMetal + request.CarriedCrystal + request.CarriedBoron;

            // Gemilerin toplam kapasitesi.
            double shipCapacity = CalculateShipCapacity(shipsInData);

            // Eğer gereğinden fazla kaynak götürülüyor ise hata dön.
            if (totalResource > shipCapacity)
                return ResponseHelper.GetError("Gemi kapasitesi yeterli değil.");

            // Gezegendeki kaynakları kontrol ediyoruz yetersiz ise hata dönüyoruz.
            if (userPlanet.Metal < request.CarriedMetal || userPlanet.Crystal < request.CarriedCrystal || userPlanet.Boron < request.CarriedBoron)
                return ResponseHelper.GetError("Yeterli hammadde bulunamadı!");

            // Gezegendeki kaynakları düşüyoruz.
            userPlanet.Metal -= request.CarriedMetal;
            userPlanet.Crystal -= request.CarriedCrystal;
            userPlanet.Boron -= request.CarriedBoron;

            // Gönderen kordinat.
            CordinateDTO senderCordinateDTO = new CordinateDTO(userPlanet.TblCordinates.GalaxyIndex, userPlanet.TblCordinates.SolarIndex, userPlanet.TblCordinates.OrderIndex);
            TblCordinates destinationCordinate = base.UnitOfWork.GetRepository<TblCordinates>().Where(x => x.GalaxyIndex == request.DestinationGalaxyIndex && x.SolarIndex == request.DestinationSolarIndex && x.OrderIndex == request.DestinationOrderIndex).Include(x => x.UserPlanet).FirstOrDefault();

            // Eğer kordinat tanımlı değil ise boştur.
            if (destinationCordinate == null)
                destinationCordinate = new TblCordinates() { GalaxyIndex = request.DestinationGalaxyIndex, SolarIndex = request.DestinationSolarIndex, OrderIndex = request.DestinationOrderIndex };

            CordinateDTO destinationCordinateDTO = new CordinateDTO(destinationCordinate.GalaxyIndex, destinationCordinate.SolarIndex, destinationCordinate.OrderIndex);

            // Uçuş süresi saniye cinsinden.
            double flyDistance = CalculateDistance(senderCordinateDTO, destinationCordinateDTO);

            // Toplam uçuş süresi.
            DateTime flyCompleteDate = base.RequestDate.AddSeconds(CalculateFlightTime(flyDistance, request.FleetSpeed, minSpeed));

            // Filoyu oluşturuyoruz.
            TblFleets fleet = base.UnitOfWork.GetRepository<TblFleets>().Add(new TblFleets
            {
                BeginDate = base.RequestDate,
                FleetActionTypeId = request.FleetType,
                DestinationCordinate = CordinateExtends.ToCordinateString(destinationCordinateDTO),
                DestinationUserPlanetId = destinationCordinate?.UserPlanetId,
                EndDate = flyCompleteDate,
                FleetData = request.Ships,
                IsReturning = false,
                SenderCordinate = CordinateExtends.ToCordinateString(senderCordinateDTO),
                SenderUserPlanetId = userPlanet.UserPlanetId,
                CarriedMetal = request.CarriedMetal,
                CarriedCrystal = request.CarriedCrystal,
                CarriedBoron = request.CarriedBoron,
                FleetSpeed = request.FleetSpeed
            });

            // Değişiklileri kayıt ediyoruz.
            base.UnitOfWork.SaveChanges();

            // Sonucu dönüyoruz.
            return ResponseHelper.GetSuccess();
        }

        private List<Tuple<Ships, int>> FleetDataToShipData(string data)
        {
            try
            {
                List<Tuple<Ships, int>> fleet = new List<Tuple<Ships, int>>();
                string[] ships = data.Split(",");
                foreach (var ship in ships)
                {
                    string[] shipData = ship.Split(":");
                    Ships shipId = (Ships)int.Parse(shipData[0]);
                    int shipQuantity = int.Parse(shipData[1]);
                    fleet.Add(new Tuple<Ships, int>(shipId, shipQuantity));
                }
                return fleet;
            }
            catch (System.Exception)
            {
                return new List<Tuple<Ships, int>>();
            }
        }

        /// <summary>
        /// Verilen gemilerden en düşük hıza sahip olanı döner.
        /// </summary>
        /// <param name="ships"></param>
        /// <returns></returns>
        public static int GetMinSpeedInFleet(IEnumerable<Ships> ships) => ships.Select(x => { return StaticData.ShipData.Find(y => y.ShipID == x).ShipSpeed; }).DefaultIfEmpty(0).Min();

        /// <summary>
        /// İki kordinat arasındaki mesafeyi hesaplar.
        /// </summary>
        /// <param name="bCordinate"></param>
        /// <param name="eCordinate"></param>
        /// <returns></returns>
        public static double CalculateDistance(CordinateDTO bCordinate, CordinateDTO eCordinate)
        {
            // Toplam tutuyoruz.
            double distance = 0;

            // Eğer farklı galaksilerde ise ekstra hesaplama yapıyoruz.
            if (bCordinate.GalaxyIndex != eCordinate.GalaxyIndex)
                distance += 20000 * Math.Abs(bCordinate.GalaxyIndex - eCordinate.GalaxyIndex);

            // Eğer aynı güneş sisteminde değil isek güneş sistemi farkını da hesaba katıyoruz.
            if (bCordinate.SolarIndex != eCordinate.SolarIndex)
                distance += 2700 + 95 * Math.Abs(bCordinate.SolarIndex - eCordinate.SolarIndex);

            // Gezegen arasındaki farkı hesaplıyoruz.
            distance += 1000 + 5 * Math.Abs(bCordinate.OrderIndex - eCordinate.OrderIndex);

            return distance;
        }

        /// <summary>
        /// Mesafe için uçuş süresini saniye cinsinden hesaplar.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="fleetSpeedRate"></param>
        /// <param name="slowestFleetSpeed"></param>
        /// <returns></returns>
        public static double CalculateFlightTime(double distance, float fleetSpeedRate, double slowestFleetSpeed)
        {
            return (10 + (3500 / fleetSpeedRate) * Math.Sqrt((10 * distance) / slowestFleetSpeed)) / 2;
        }

        /// <summary>
        /// Filo kapasitesini hesaplar.
        /// </summary>
        /// <param name="ships"></param>
        /// <returns></returns>
        public static double CalculateShipCapacity(IEnumerable<Tuple<Ships, int>> ships) => ships.Select(x => { return StaticData.ShipData.Find(y => y.ShipID == x.Item1).CargoCapacity * x.Item2; }).DefaultIfEmpty(0).Sum();

        /// <summary>
        /// Filonun yakıt kapasitesini hesaplar.
        /// </summary>
        /// <param name="ships">İlk parametre gemi bilgisi. 2 parametere mesafe.</param>
        /// <param name="distance"></param>
        /// <param name="fleetSpeedRate"></param>
        /// <returns></returns>
        public static double CalculateFuelCost(IEnumerable<Tuple<Ships, int>> ships, double distance, double fleetSpeedRate)
        {

            return 1 + Math.Round(ships.Select(x =>
            {
                // Gemi bilgisini buluyoruz.
                ShipDTO shipData = StaticData.ShipData.Find(y => y.ShipID == x.Item1);

                // Temel değerlerine bakarak yakıtını hesaplıyoruz.
                return Math.Round(shipData.BaseFuelt * distance / 35000 * TamKare(fleetSpeedRate, 1), 5) * x.Item2;

            }).DefaultIfEmpty(0).Sum(), 0);
        }

        /// <summary>
        /// Tam karesini alır.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static double TamKare(double first, double second) => Math.Pow(first, 2) + (2 * first * second) + Math.Pow(second, 2);

    }
}
