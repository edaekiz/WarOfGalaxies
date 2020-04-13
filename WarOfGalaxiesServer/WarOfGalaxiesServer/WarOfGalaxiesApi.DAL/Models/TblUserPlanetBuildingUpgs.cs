using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanetBuildingUpgs
    {
        public int UserPlanetBuildingUpgId { get; set; }
        public int BuildingLevelId { get; set; }
        public int PlanetId { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
