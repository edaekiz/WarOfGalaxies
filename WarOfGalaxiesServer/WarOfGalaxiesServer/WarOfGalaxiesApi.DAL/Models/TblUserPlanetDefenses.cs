using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanetDefenses
    {
        public int UserPlanetDefenseId { get; set; }
        public int UserPlanetId { get; set; }
        public int DefenseId { get; set; }
        public int DefenseCount { get; set; }

        public virtual TblDefenses Defense { get; set; }
        public virtual TblUserPlanets UserPlanet { get; set; }
    }
}
