using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblCordinateTypes
    {
        public TblCordinateTypes()
        {
            TblCordinates = new HashSet<TblCordinates>();
        }

        public int CordinateTypeId { get; set; }
        public string CordinateTypeName { get; set; }

        public virtual ICollection<TblCordinates> TblCordinates { get; set; }
    }
}
