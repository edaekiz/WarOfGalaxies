using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanetBuildingUpgs
    {
        public int UserPlanetBuildingUpgId { get; set; }
        public int UserPlanetId { get; set; }
        public int BuildingId { get; set; }
        public int BuildingLevel { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual TblBuildings Building { get; set; }
        public virtual TblUserPlanets UserPlanet { get; set; }
    }
}
