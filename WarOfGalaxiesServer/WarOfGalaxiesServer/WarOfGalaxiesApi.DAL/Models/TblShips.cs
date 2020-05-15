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

        public virtual ICollection<TblUserPlanetShipProgs> TblUserPlanetShipProgs { get; set; }
        public virtual ICollection<TblUserPlanetShips> TblUserPlanetShips { get; set; }
    }
}
