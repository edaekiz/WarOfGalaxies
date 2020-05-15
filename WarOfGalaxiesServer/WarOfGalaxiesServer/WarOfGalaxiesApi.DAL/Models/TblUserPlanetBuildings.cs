using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanetBuildings
    {
        public int UserPlanetBuildingId { get; set; }
        public int UserPlanetId { get; set; }
        public int BuildingId { get; set; }
        public int BuildingLevel { get; set; }

        public virtual TblBuildings Building { get; set; }
        public virtual TblUserPlanets UserPlanet { get; set; }
    }
}
