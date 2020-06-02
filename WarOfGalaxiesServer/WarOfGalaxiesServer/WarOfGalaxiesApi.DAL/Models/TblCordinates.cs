using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblCordinates
    {
        public int CordinateId { get; set; }
        public int CordinateTypeId { get; set; }
        public int GalaxyIndex { get; set; }
        public int SolarIndex { get; set; }
        public int OrderIndex { get; set; }
        public double Metal { get; set; }
        public double Crystal { get; set; }
        public double Boron { get; set; }
        public int? UserPlanetId { get; set; }

        public virtual TblCordinateTypes CordinateType { get; set; }
        public virtual TblUserPlanets UserPlanet { get; set; }
    }
}
