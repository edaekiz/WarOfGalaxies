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
        public double BaseCostMetal { get; set; }
        public double BaseCostCrystal { get; set; }
        public double BaseCostBoron { get; set; }
        public double BaseValue { get; set; }
        public double BuildingUpgradeCostRate { get; set; }

        public virtual ICollection<TblUserPlanetBuildingUpgs> TblUserPlanetBuildingUpgs { get; set; }
        public virtual ICollection<TblUserPlanetBuildings> TblUserPlanetBuildings { get; set; }
    }
}
