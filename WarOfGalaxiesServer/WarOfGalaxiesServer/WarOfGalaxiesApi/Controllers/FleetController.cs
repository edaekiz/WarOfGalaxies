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
                    EndLeftTime = (x.EndDate - RequestDate).TotalSeconds
                }).ToList();

            return ResponseHelper.GetSuccess(lastFleets);
        }

        [HttpPost("FlyNewFleet")]
        [Description("Gezegenden filo çıkarma.")]
        public ApiResult FlyNewFleet(SendFleetFromPlanetDTO request)
        {
            // Kullanıcının gezegeni.
            TblUserPlanets userPlanet = base.UnitOfWork.GetRepository<TblUserPlanets>().FirstOrDefault(x => x.UserPlanetId == request.UserPlanetId && x.UserId == base.DBUser.UserId);

            // Gezegen yok ise hata dön.
            if (userPlanet == null)
                return ResponseHelper.GetError("Gezegeni bulamadık!");

            // Kullanıcının filosunu alıyoruz.
            List<TblUserPlanetShips> userPlanetShips = base.UnitOfWork.GetRepository<TblUserPlanetShips>().Where(x => x.UserPlanetId == request.UserPlanetId).ToList();

            // Gelen gemilerin bilsini anlaşılır formata çeviriyoruz.
            Dictionary<Ships, int> shipsInData = FleetDataToShipData(request.Ships);

            // Eğer hiç gemi yok ise yola çıkmıyoruz.
            if (shipsInData.Count == 0)
                return ResponseHelper.GetError("Geçersiz veri tipi yada hiç gemi bulunamadı!");

            // Her bir gemiyi tersaneden düşüyoruz.
            foreach (KeyValuePair<Ships, int> ship in shipsInData)
            {
                // Kullanıcıya ait gemiyi buluyoruz.
                TblUserPlanetShips planetShip = userPlanetShips.Find(x => x.ShipId == (int)ship.Key);

                // Eğer gemi yok ise yada miktarı az ise hata dönüyoruz.
                if (planetShip == null || planetShip.ShipCount < ship.Value)
                    return ResponseHelper.GetError("Yeterli gemi yoktu!");

                // Tersaneden düşüyoruz.
                planetShip.ShipCount -= ship.Value;

                // Eğer başka gemi kalmamış ise kaydı siliyoruz.
                if (planetShip.ShipCount <= 0)
                    base.UnitOfWork.GetRepository<TblUserPlanetShips>().Delete(planetShip);
            }

            // Minimum gemi hızı.
            int minSpeed = GetMinSpeedInFleet(shipsInData.Select(x => x.Key));

            // Kullanıcının toplam göndereceği miktar.
            double totalResource = request.Metal + request.Crystal + request.Boron;

            // Gemilerin toplam kapasitesi.
            double shipCapacity = CalculateShipCapacity(shipsInData.Select(x => x.Key));

            // Eğer gereğinden fazla kaynak götürülüyor ise hata dön.
            if (totalResource > shipCapacity)
                return ResponseHelper.GetError("Gemi kapasitesi yeterli değil.");

            // Gezegendeki kaynakları kontrol ediyoruz yetersiz ise hata dönüyoruz.
            if (userPlanet.Metal < request.Metal || userPlanet.Crystal < request.Crystal || userPlanet.Boron < request.Boron)
                return ResponseHelper.GetError("Yeterli hammadde bulunamadı!");

            // Gezegendeki kaynakları düşüyoruz.
            userPlanet.Metal -= request.Metal;
            userPlanet.Crystal -= request.Crystal;
            userPlanet.Boron -= request.Boron;

            TblCordinates senderCordinate = base.UnitOfWork.GetRepository<TblCordinates>().FirstOrDefault(x => x.UserPlanetId == userPlanet.UserPlanetId);
            CordinateDTO senderCordinateDTO = new CordinateDTO(senderCordinate.GalaxyIndex, senderCordinate.SolarIndex, senderCordinate.OrderIndex);
            TblCordinates destinationCordinate = base.UnitOfWork.GetRepository<TblCordinates>().FirstOrDefault(x => x.GalaxyIndex == request.GalaxyIndex && x.SolarIndex == request.SolarIndex && x.OrderIndex == request.OrderIndex);
            CordinateDTO destinationCordinateDTO = new CordinateDTO(destinationCordinate.GalaxyIndex, destinationCordinate.SolarIndex, destinationCordinate.OrderIndex);

            DateTime flyDate = base.RequestDate.AddSeconds(CalculateFlightTime(senderCordinateDTO, destinationCordinateDTO, 100, minSpeed));

            // Filoyu oluşturuyoruz.
            TblFleets fleet = base.UnitOfWork.GetRepository<TblFleets>().Add(new TblFleets
            {
                BeginDate = base.RequestDate,
                FleetActionTypeId = request.FleetType,
                DestinationCordinate = CordinateExtends.ToCordinateString(destinationCordinateDTO),
                DestinationUserPlanetId = destinationCordinate?.UserPlanetId,
                EndDate = flyDate,
                FleetData = request.Ships,
                IsReturning = false,
                SenderCordinate = CordinateExtends.ToCordinateString(senderCordinateDTO),
                SenderUserPlanetId = userPlanet.UserPlanetId,
                CarriedMetal = request.Metal,
                CarriedCrystal = request.Crystal,
                CarriedBoron = request.Boron
            });

            base.UnitOfWork.SaveChanges();

            return null;
        }

        private Dictionary<Ships, int> FleetDataToShipData(string data)
        {
            try
            {
                Dictionary<Ships, int> fleet = new Dictionary<Ships, int>();
                string[] ships = data.Split(",");
                foreach (var ship in ships)
                {
                    string[] shipData = ship.Split(":");
                    Ships shipId = (Ships)int.Parse(shipData[0]);
                    int shipQuantity = int.Parse(shipData[1]);
                    fleet.Add(shipId, shipQuantity);
                }
                return fleet;
            }
            catch (System.Exception)
            {
                return new Dictionary<Ships, int>();
            }
        }

        /// <summary>
        /// Verilen gemilerden en düşük hıza sahip olanı döner.
        /// </summary>
        /// <param name="ships"></param>
        /// <returns></returns>
        private int GetMinSpeedInFleet(IEnumerable<Ships> ships) => ships.Select(x => { return StaticData.ShipData.Find(y => y.ShipID == x).ShipSpeed; }).DefaultIfEmpty(0).Min();

        private double CalculateFlightTime(CordinateDTO bCordinate, CordinateDTO eCordinate, int fleetSpeedRate, int slowestFleetSpeed)
        {
            int galaxyDif = 20000 * Math.Abs(bCordinate.GalaxyIndex - eCordinate.GalaxyIndex);
            int solarDif = 2700 + 95 * Math.Abs(bCordinate.SolarIndex - eCordinate.SolarIndex);
            int orderDif = 1000 + 5 * Math.Abs(bCordinate.OrderIndex - eCordinate.OrderIndex);
            int distance = galaxyDif + solarDif + orderDif;
            return (10 + (3500 / fleetSpeedRate) * Math.Sqrt((10 * distance) / slowestFleetSpeed)) / 1;
        }

        public double CalculateShipCapacity(IEnumerable<Ships> ships) => ships.Select(x => { return StaticData.ShipData.Find(y => y.ShipID == x).CargoCapacity; }).DefaultIfEmpty(0).Sum();

    }
}
