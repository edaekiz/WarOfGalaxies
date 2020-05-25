using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Extends
{
    public static class FleetExtends
    {
        /// <summary>
        /// Verilen gemilerden en düşük hıza sahip olanı döner.
        /// </summary>
        /// <param name="ships"></param>
        /// <returns></returns>
        public static double GetMinSpeedInFleet(IEnumerable<Ships> ships) => ships.Select(x => { return DataController.DC.GetShip(x).ShipSpeed; }).DefaultIfEmpty(0).Min();

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
            return (10 + (3500 / fleetSpeedRate) * Math.Sqrt((10 * distance) / slowestFleetSpeed)) / DataController.DC.UniverseFleetSpeed;
        }

        /// <summary>
        /// Filo kapasitesini hesaplar.
        /// </summary>
        /// <param name="ships"></param>
        /// <returns></returns>
        public static double CalculateShipCapacity(IEnumerable<Tuple<Ships, int>> ships) => ships.Select(x => { return DataController.DC.GetShip(x.Item1).CargoCapacity * x.Item2; }).DefaultIfEmpty(0).Sum();

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
                ShipDataDTO shipData = DataController.DC.GetShip(x.Item1);

                // Temel değerlerine bakarak yakıtını hesaplıyoruz.
                return Math.Round(shipData.ShipFuelt * distance / 35000 * MathExtends.TamKare(fleetSpeedRate, 1), 5) * x.Item2;

            }).DefaultIfEmpty(0).Sum(), 0);
        }

        public static List<Tuple<Ships, int>> FleetDataToShipData(string data)
        {
            try
            { 
                List<Tuple<Ships, int>> fleet = new List<Tuple<Ships, int>>();
                string[] ships = data.Split(new string[] { "," }, StringSplitOptions.None);
                foreach (var ship in ships)
                {
                    string[] shipData = ship.Split(new string[] { ":" }, StringSplitOptions.None);
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

        public static string ShipDataToStringData(IEnumerable<Tuple<Ships, int>> fleet) => string.Join(",", fleet.Select(x => $"{(int)x.Item1}:{x.Item2}"));

    }
}
