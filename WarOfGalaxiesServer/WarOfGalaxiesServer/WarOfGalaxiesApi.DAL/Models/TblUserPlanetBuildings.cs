using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanetBuildings
    {
        public int BuildingId { get; set; }
        public int PlanetId { get; set; }
        public int BuildingLevel { get; set; }
    }
}
