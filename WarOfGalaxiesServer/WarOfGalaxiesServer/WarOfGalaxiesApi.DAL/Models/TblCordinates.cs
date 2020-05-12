using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblCordinates
    {
        public int CordinateId { get; set; }
        public int GalaxyIndex { get; set; }
        public int SolarIndex { get; set; }
        public int OrderIndex { get; set; }
        public int? UserPlanetId { get; set; }
    }
}
