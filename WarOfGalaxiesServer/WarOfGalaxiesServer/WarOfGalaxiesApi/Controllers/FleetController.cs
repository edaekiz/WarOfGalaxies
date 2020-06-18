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
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public class FleetController : MainController
    {
        public FleetController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
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
                    BeginPassedTime = (RequestDate - x.BeginDate).TotalSeconds,
                    EndLeftTime = (x.EndDate - RequestDate).TotalSeconds,
                    CarriedBoron = x.CarriedBoron,
                    CarriedCrystal = x.CarriedCrystal,
                    CarriedMetal = x.CarriedMetal,
                    DestinationPlanetTypeId = x.DestinationUserPlanetId.HasValue ? x.DestinationUserPlanet.PlanetType : 0,
                    SenderPlanetTypeId = x.SenderUserPlanet.PlanetType,
                    FleetData = x.FleetData,
                    ReturnFleetId = x.ReturnFleetId,
                    FleetSpeed = x.FleetSpeed
                }).ToList();

            return ResponseHelper.GetSuccess(lastFleets);
        }

        [HttpPost("GetFleetById")]
        [Description("Verilen filo hareketlerini döner.")]
        public ApiResult GetFleetById(GetLastFleetsDTO request)
        {
            // Kullanıcıya gönderilen yada kullanıcıdan gönderilen filoların listesini alıyoruz.
            FleetDTO lastFleet = base.UnitOfWork.GetRepository<TblFleets>()
                .Where(x => x.FleetId == request.LastFleetId)
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
                    BeginPassedTime = (RequestDate - x.BeginDate).TotalSeconds,
                    EndLeftTime = (x.EndDate - RequestDate).TotalSeconds,
                    CarriedBoron = x.CarriedBoron,
                    CarriedCrystal = x.CarriedCrystal,
                    CarriedMetal = x.CarriedMetal,
                    DestinationPlanetTypeId = x.DestinationUserPlanetId.HasValue ? x.DestinationUserPlanet.PlanetType : 0,
                    SenderPlanetTypeId = x.SenderUserPlanet.PlanetType,
                    FleetData = x.FleetData,
                    ReturnFleetId = x.ReturnFleetId,
                    FleetSpeed = x.FleetSpeed
                }).FirstOrDefault();

            return ResponseHelper.GetSuccess(lastFleet);
        }

        [HttpPost("FlyNewFleet")]
        [Description("Gezegenden filo çıkarma.")]
        public ApiResult FlyNewFleet(SendFleetFromPlanetDTO request)
        {
            #region Gönderen gezegeni.

            // Kullanıcının gezegeni.
            TblUserPlanets userPlanet = base.UnitOfWork.GetRepository<TblUserPlanets>()
                .Where(x => x.UserPlanetId == request.SenderUserPlanetId && x.UserId == DBUser.UserId)
                .Include(x => x.TblCordinates)
                .FirstOrDefault();

            // Gezegen yok ise hata dön.
            if (userPlanet == null)
                return ResponseHelper.GetError("Gezegeni bulamadık!");

            #endregion

            #region Gönderen kordinatı.

            // Gönderen kordinat.
            CordinateDTO senderCordinateDTO = new CordinateDTO(userPlanet.TblCordinates.GalaxyIndex, userPlanet.TblCordinates.SolarIndex, userPlanet.TblCordinates.OrderIndex);

            #endregion

            #region Hedef Kordinatı

            // Eğer geçersiz bir kordinat ise geri dön.
            if (!base.StaticValues.IsValidCordinate(request.DestinationGalaxyIndex, request.DestinationSolarIndex, request.DestinationOrderIndex))
                return ResponseHelper.GetError("Kordinat bulunamadı!");

            // Hedef kordinat.
            TblCordinates destinationCordinate = base.UnitOfWork.GetRepository<TblCordinates>().Where(x => x.GalaxyIndex == request.DestinationGalaxyIndex && x.SolarIndex == request.DestinationSolarIndex && x.OrderIndex == request.DestinationOrderIndex).Include(x => x.UserPlanet).FirstOrDefault();

            // Eğer kordinat tanımlı değil ise boştur.
            if (destinationCordinate == null)
                destinationCordinate = new TblCordinates() { GalaxyIndex = request.DestinationGalaxyIndex, SolarIndex = request.DestinationSolarIndex, OrderIndex = request.DestinationOrderIndex };

            CordinateDTO destinationCordinateDTO = new CordinateDTO(destinationCordinate.GalaxyIndex, destinationCordinate.SolarIndex, destinationCordinate.OrderIndex);

            #endregion

            // Filo hızının geçerliliğini kontrol ediyoruz.
            if (request.FleetSpeed < 0.01f || request.FleetSpeed > 1.0f)
                return ResponseHelper.GetError("Geçersiz filo hızı!");

            // Gelen gemilerin bilsini anlaşılır formata çeviriyoruz.
            List<Tuple<Ships, int>> shipsInData = FleetDataToShipData(request.Ships);

            // Eğer hiç gemi yok ise yola çıkmıyoruz.
            if (shipsInData.Count == 0)
                return ResponseHelper.GetError("Geçersiz veri tipi yada hiç gemi bulunamadı!");

            #region Seçilen aksiyon türüne özel kontroller.

            switch ((FleetTypes)request.FleetType)
            {
                case FleetTypes.Casusluk:
                    {
                        // Casus sondasını arıyoruz.
                        Tuple<Ships, int> spySolar = shipsInData.Find(x => x.Item1 == Ships.CasusSondası);

                        // Eğer yok ise ozaman hata dönüyoruz. Çünkü casusluk da en az bir gemi olmalı.
                        if (spySolar == null || spySolar.Item2 <= 0)
                            return ResponseHelper.GetError("En az bir casus sondası olmak zorunda.");

                        // Hedefte bir gezegen var mı?
                        if (destinationCordinate.UserPlanet == null)
                            return ResponseHelper.GetError("Hedef konumda bir gezegen olmak zorunda.");
                    }
                    break;
                case FleetTypes.Sömürgeleştir:
                    break;
                case FleetTypes.Sök:

                    // Sok hareketi için geri dönüşümcü olmak zorunda.
                    Tuple<Ships, int> garbageCollector = shipsInData.Find(x => x.Item1 == Ships.GeriDönüşümcü);

                    // Eğer yok ise ozaman hata dönüyoruz. Çünkü casusluk da en az bir gemi olmalı.
                    if (garbageCollector == null || garbageCollector.Item2 <= 0)
                        return ResponseHelper.GetError("En az bir casus sondası olmak zorunda.");

                    break;
                default:
                    {
                        // Hedefte bir gezegen var mı?
                        if (destinationCordinate.UserPlanet == null)
                            return ResponseHelper.GetError("Hedef konumda bir gezegen olmak zorunda.");
                    }
                    break;
            }

            #endregion

            #region Kaynak onaylama

            // Doğrulama tamamlandı mı?
            VerifyController.VerifyAllFleets(this, new VerifyResourceDTO { UserPlanetID = request.SenderUserPlanetId });

            #endregion

            #region Gemileri tersaneden düşüyoruz.

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

            #endregion

            #region Taşınan kaynaklar, Kapasite ve filo hızı.

            // Minimum gemi hızı.
            double minSpeed = GetMinSpeedInFleet(shipsInData.Select(x => x.Item1));

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

            #endregion

            #region Uçuş Mesafe ve Süresi.

            // Uçuş süresi saniye cinsinden.
            double flyDistance = CalculateDistance(senderCordinateDTO, destinationCordinateDTO);

            double totalFlightTime = CalculateFlightTime(flyDistance, request.FleetSpeed, minSpeed);

            // Toplam uçuş süresi.
            DateTime arriveDate = base.RequestDate.AddSeconds(totalFlightTime / 2);

            // Dönüş tarihi.
            DateTime returnDate = arriveDate.AddSeconds(totalFlightTime / 2);

            #endregion

            // Filoyu oluşturuyoruz.
            TblFleets fleet = base.UnitOfWork.GetRepository<TblFleets>().Add(new TblFleets
            {
                BeginDate = base.RequestDate,
                FleetActionTypeId = request.FleetType,
                DestinationCordinate = CordinateExtends.ToCordinateString(destinationCordinateDTO),
                DestinationUserPlanetId = request.FleetType == (int)FleetTypes.Sök ? null : destinationCordinate?.UserPlanetId,
                EndDate = arriveDate,
                FleetData = request.Ships,
                SenderCordinate = CordinateExtends.ToCordinateString(senderCordinateDTO),
                SenderUserPlanetId = userPlanet.UserPlanetId,
                CarriedMetal = request.CarriedMetal,
                CarriedCrystal = request.CarriedCrystal,
                CarriedBoron = request.CarriedBoron,
                FleetSpeed = request.FleetSpeed
            });

            // Dönüş filosunu oluşturuyoruz.
            TblFleets fleetReturn = base.UnitOfWork.GetRepository<TblFleets>().Add(new TblFleets
            {
                BeginDate = arriveDate,
                FleetActionTypeId = request.FleetType,
                DestinationCordinate = fleet.SenderCordinate,
                DestinationUserPlanetId = fleet.SenderUserPlanetId,
                EndDate = returnDate,
                FleetData = request.Ships,
                SenderCordinate = fleet.DestinationCordinate,
                SenderUserPlanetId = fleet.DestinationUserPlanetId,
                CarriedMetal = request.CarriedMetal,
                CarriedCrystal = request.CarriedCrystal,
                CarriedBoron = request.CarriedBoron,
                FleetSpeed = request.FleetSpeed
            });

            fleet.ReturnFleet = fleetReturn;

            // Değişiklileri kayıt ediyoruz.
            base.UnitOfWork.SaveChanges();

            // Sonucu dönüyoruz.
            return ResponseHelper.GetSuccess();
        }

        [HttpPost("CallBackFlyFleet")]
        [Description("Gidiş yolunda olan bir filoyu geri çağırır.")]
        public ApiResult CallBackFlyFleet(CallbackFleetDTO request)
        {
            // Hedefe giden gemiler sadece döndürülebilir bir de sadece kullanıcıya ait olan gemileri geri çağırabiliriz.
            TblFleets fleet = base.UnitOfWork.GetRepository<TblFleets>()
                .Where(x => x.FleetId == request.FleetId && x.SenderUserPlanet.UserId == DBUser.UserId && x.ReturnFleetId.HasValue)
                .Include(x => x.ReturnFleet)
                .FirstOrDefault();

            // Filo yok ise geri dönüyoruz.
            if (fleet == null)
                return ResponseHelper.GetError("Filo bulunamadı!");

            // Toplam geçmiş olan süreyi alıyoruz.
            double passedDate = (RequestDate - fleet.BeginDate).TotalSeconds;

            // Başlangıç tarihimizi güncelliyoruz.
            fleet.ReturnFleet.BeginDate = fleet.BeginDate.AddSeconds(passedDate);

            // Dönüş süresi kısalmış oluyor güncelliyoruz.
            fleet.ReturnFleet.EndDate = fleet.ReturnFleet.BeginDate.AddSeconds(passedDate);

            // Gidiş filosunu siliyoruz.
            base.UnitOfWork.GetRepository<TblFleets>().Delete(fleet);

            // Değişiklikleri kayıt ediyoruz.
            base.UnitOfWork.SaveChanges();

            // Başarılı sonucunu dönüyoruz.
            return ResponseHelper.GetSuccess();
        }

        /// <summary>
        /// Gemi listesini string formata çevirir.
        /// </summary>
        /// <param name="fleet"></param>
        /// <returns></returns>
        public static string ShipDataToStringData(IEnumerable<Tuple<Ships, int>> fleet) => string.Join(",", fleet.Select(x => $"{(int)x.Item1}:{x.Item2}"));

        /// <summary>
        /// Gemi datasını listeye dönüştürür.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<Tuple<Ships, int>> FleetDataToShipData(string data)
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
        public double GetMinSpeedInFleet(IEnumerable<Ships> ships) => ships.Select(x => { return StaticValues.GetShip(x).ShipSpeed; }).DefaultIfEmpty(0).Min();

        /// <summary>
        /// İki kordinat arasındaki mesafeyi hesaplar.
        /// </summary>
        /// <param name="bCordinate"></param>
        /// <param name="eCordinate"></param>
        /// <returns></returns>
        public double CalculateDistance(CordinateDTO bCordinate, CordinateDTO eCordinate)
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
        public double CalculateFlightTime(double distance, float fleetSpeedRate, double slowestFleetSpeed)
        {
            return (10 + (3500 / fleetSpeedRate) * Math.Sqrt((10 * distance) / slowestFleetSpeed)) / StaticValues.UniverseFleetSpeed;
        }

        /// <summary>
        /// Filo kapasitesini hesaplar.
        /// </summary>
        /// <param name="ships"></param>
        /// <returns></returns>
        public double CalculateShipCapacity(IEnumerable<Tuple<Ships, int>> ships) => ships.Select(x => { return StaticValues.GetShip(x.Item1).CargoCapacity * x.Item2; }).DefaultIfEmpty(0).Sum();

        /// <summary>
        /// Filonun yakıt kapasitesini hesaplar.
        /// </summary>
        /// <param name="ships">İlk parametre gemi bilgisi. 2 parametere mesafe.</param>
        /// <param name="distance"></param>
        /// <param name="fleetSpeedRate"></param>
        /// <returns></returns>
        public double CalculateFuelCost(IEnumerable<Tuple<Ships, int>> ships, double distance, double fleetSpeedRate)
        {

            return 1 + Math.Round(ships.Select(x =>
            {
                // Gemi bilgisini buluyoruz.
                TblShips shipData = StaticValues.GetShip(x.Item1);

                // Temel değerlerine bakarak yakıtını hesaplıyoruz.
                return Math.Round(shipData.ShipFuelt * distance / 35000 * TamKare(fleetSpeedRate, 1), 5) * x.Item2;

            }).DefaultIfEmpty(0).Sum(), 0);
        }

        /// <summary>
        /// Tam karesini alır.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public double TamKare(double first, double second) => Math.Pow(first, 2) + (2 * first * second) + Math.Pow(second, 2);

    }
}
