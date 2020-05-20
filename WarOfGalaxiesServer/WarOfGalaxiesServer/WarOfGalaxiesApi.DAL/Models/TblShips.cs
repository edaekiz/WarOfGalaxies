using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblShips
    {
        public TblShips()
        {
            TblUserPlanetShipProgs = new HashSet<TblUserPlanetShipProgs>();
            TblUserPlanetShips = new HashSet<TblUserPlanetShips>();
        }

        public int ShipId { get; set; }
        public string ShipName { get; set; }
        public double CostMetal { get; set; }
        public double CostCrystal { get; set; }
        public double CostBoron { get; set; }
        public double ShipSpeed { get; set; }
        public double ShipFuelt { get; set; }
        public double CargoCapacity { get; set; }

        public virtual ICollection<TblUserPlanetShipProgs> TblUserPlanetShipProgs { get; set; }
        public virtual ICollection<TblUserPlanetShips> TblUserPlanetShips { get; set; }
    }
}
