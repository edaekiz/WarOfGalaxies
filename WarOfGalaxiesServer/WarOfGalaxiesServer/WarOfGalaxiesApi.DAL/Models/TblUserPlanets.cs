using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserPlanets
    {
        public int UserPlanetId { get; set; }
        public int UserId { get; set; }
        public int PlanetId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
