using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblBuildings
    {
        public TblBuildings()
        {
            TblUserPlanetBuildingUpgs = new HashSet<TblUserPlanetBuildingUpgs>();
            TblUserPlanetBuildings = new HashSet<TblUserPlanetBuildings>();
        }

        public int BuildingId { get; set; }
        public string BuildingName { get; set; }

        public virtual ICollection<TblUserPlanetBuildingUpgs> TblUserPlanetBuildingUpgs { get; set; }
        public virtual ICollection<TblUserPlanetBuildings> TblUserPlanetBuildings { get; set; }
    }
}
