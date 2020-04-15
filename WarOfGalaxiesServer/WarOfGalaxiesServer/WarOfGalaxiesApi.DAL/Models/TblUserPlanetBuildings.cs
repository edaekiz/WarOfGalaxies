using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanetBuildings
    {
        public int UserPlanetId { get; set; }
        public int UserId { get; set; }
        public int BuildingId { get; set; }
        public int BuildingLevel { get; set; }
    }
}
