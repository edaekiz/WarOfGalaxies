using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanets
    {
        public int UserPlanetId { get; set; }
        public int UserId { get; set; }
        public string PlanetCordinate { get; set; }
        public int PlanetType { get; set; }
        public string PlanetName { get; set; }
        public int Metal { get; set; }
        public int Crystal { get; set; }
        public int Boron { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
