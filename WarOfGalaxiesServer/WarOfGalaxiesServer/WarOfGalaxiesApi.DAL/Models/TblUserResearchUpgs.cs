using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserResearchUpgs
    {
        public int UserResearchUpgId { get; set; }
        public int UserId { get; set; }
        public int UserPlanetId { get; set; }
        public int ResearchId { get; set; }
        public int ResearchTargetLevel { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual TblResearches Research { get; set; }
        public virtual TblUsers User { get; set; }
        public virtual TblUserPlanets UserPlanet { get; set; }
    }
}
