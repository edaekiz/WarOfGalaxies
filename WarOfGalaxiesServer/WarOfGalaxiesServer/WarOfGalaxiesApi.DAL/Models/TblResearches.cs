using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblResearches
    {
        public TblResearches()
        {
            TblUserResearchUpgs = new HashSet<TblUserResearchUpgs>();
            TblUserResearches = new HashSet<TblUserResearches>();
        }

        public int ResearchId { get; set; }
        public string ResearchName { get; set; }
        public double BaseCostMetal { get; set; }
        public double BaseCostCrystal { get; set; }
        public double BaseCostBoron { get; set; }
        public double ResearchValue { get; set; }

        public virtual ICollection<TblUserResearchUpgs> TblUserResearchUpgs { get; set; }
        public virtual ICollection<TblUserResearches> TblUserResearches { get; set; }
    }
}
