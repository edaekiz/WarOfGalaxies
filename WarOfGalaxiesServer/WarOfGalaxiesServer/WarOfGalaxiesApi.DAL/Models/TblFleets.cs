using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblFleets
    {
        public int FleetId { get; set; }
        public int FleetActionTypeId { get; set; }
        public int SenderUserPlanetId { get; set; }
        public string SenderCordinate { get; set; }
        public int? DestinationUserPlanetId { get; set; }
        public string DestinationCordinate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsReturning { get; set; }
        public string FleetData { get; set; }
        public double CarriedMetal { get; set; }
        public double CarriedCrystal { get; set; }
        public double CarriedBoron { get; set; }
        public double FleetSpeed { get; set; }

        public virtual TblUserPlanets DestinationUserPlanet { get; set; }
        public virtual TblFleetActionTypes FleetActionType { get; set; }
        public virtual TblUserPlanets SenderUserPlanet { get; set; }
    }
}
