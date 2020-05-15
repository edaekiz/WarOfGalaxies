using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblDefenses
    {
        public TblDefenses()
        {
            TblUserPlanetDefenseProgs = new HashSet<TblUserPlanetDefenseProgs>();
            TblUserPlanetDefenses = new HashSet<TblUserPlanetDefenses>();
        }

        public int DefenseId { get; set; }
        public string DefenseName { get; set; }

        public virtual ICollection<TblUserPlanetDefenseProgs> TblUserPlanetDefenseProgs { get; set; }
        public virtual ICollection<TblUserPlanetDefenses> TblUserPlanetDefenses { get; set; }
    }
}
