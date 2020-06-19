using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblTechnology
    {
        public int TechnologyCategoryId { get; set; }
        public int IndexId { get; set; }
        public int RequiredIndexId { get; set; }
        public int RequiredLevel { get; set; }
        public int RequiredTechnologyCategoryId { get; set; }

        public virtual TblTechnologyCategories RequiredTechnologyCategory { get; set; }
        public virtual TblTechnologyCategories TechnologyCategory { get; set; }
    }
}
