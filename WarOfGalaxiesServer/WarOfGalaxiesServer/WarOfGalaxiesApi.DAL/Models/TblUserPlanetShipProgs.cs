using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanetShipProgs
    {
        public int UserPlanetShipProgId { get; set; }
        public int UserPlanetId { get; set; }
        public int ShipId { get; set; }
        public int ShipCount { get; set; }
        public DateTime? LastVerifyDate { get; set; }

        public virtual TblShips Ship { get; set; }
        public virtual TblUserPlanets UserPlanet { get; set; }
    }
}
