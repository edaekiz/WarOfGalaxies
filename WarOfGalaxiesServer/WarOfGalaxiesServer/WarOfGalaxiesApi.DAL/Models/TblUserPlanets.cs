using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanets
    {
        public TblUserPlanets()
        {
            TblFleetsDestinationUserPlanet = new HashSet<TblFleets>();
            TblFleetsSenderUserPlanet = new HashSet<TblFleets>();
            TblUserPlanetBuildingUpgs = new HashSet<TblUserPlanetBuildingUpgs>();
            TblUserPlanetBuildings = new HashSet<TblUserPlanetBuildings>();
            TblUserPlanetDefenseProgs = new HashSet<TblUserPlanetDefenseProgs>();
            TblUserPlanetDefenses = new HashSet<TblUserPlanetDefenses>();
            TblUserPlanetShipProgs = new HashSet<TblUserPlanetShipProgs>();
            TblUserPlanetShips = new HashSet<TblUserPlanetShips>();
            TblUserResearchUpgs = new HashSet<TblUserResearchUpgs>();
        }

        public int UserPlanetId { get; set; }
        public int UserId { get; set; }
        public int PlanetType { get; set; }
        public string PlanetName { get; set; }
        public double Metal { get; set; }
        public double Crystal { get; set; }
        public double Boron { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public virtual TblUsers User { get; set; }
        public virtual TblCordinates TblCordinates { get; set; }
        public virtual ICollection<TblFleets> TblFleetsDestinationUserPlanet { get; set; }
        public virtual ICollection<TblFleets> TblFleetsSenderUserPlanet { get; set; }
        public virtual ICollection<TblUserPlanetBuildingUpgs> TblUserPlanetBuildingUpgs { get; set; }
        public virtual ICollection<TblUserPlanetBuildings> TblUserPlanetBuildings { get; set; }
        public virtual ICollection<TblUserPlanetDefenseProgs> TblUserPlanetDefenseProgs { get; set; }
        public virtual ICollection<TblUserPlanetDefenses> TblUserPlanetDefenses { get; set; }
        public virtual ICollection<TblUserPlanetShipProgs> TblUserPlanetShipProgs { get; set; }
        public virtual ICollection<TblUserPlanetShips> TblUserPlanetShips { get; set; }
        public virtual ICollection<TblUserResearchUpgs> TblUserResearchUpgs { get; set; }
    }
}
