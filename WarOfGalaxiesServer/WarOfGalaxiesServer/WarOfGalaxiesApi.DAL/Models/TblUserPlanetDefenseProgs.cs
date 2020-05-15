using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanetDefenseProgs
    {
        public int UserPlanetDefenseProgId { get; set; }
        public int UserPlanetId { get; set; }
        public int DefenseId { get; set; }
        public int DefenseCount { get; set; }
        public DateTime? LastVerifyDate { get; set; }

        public virtual TblDefenses Defense { get; set; }
        public virtual TblUserPlanets UserPlanet { get; set; }
    }
}
