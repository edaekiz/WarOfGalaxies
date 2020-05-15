using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserResearches
    {
        public int UserResearchId { get; set; }
        public int UserId { get; set; }
        public int ResearchId { get; set; }
        public int ResearchLevel { get; set; }

        public virtual TblResearches Research { get; set; }
        public virtual TblUsers User { get; set; }
    }
}
