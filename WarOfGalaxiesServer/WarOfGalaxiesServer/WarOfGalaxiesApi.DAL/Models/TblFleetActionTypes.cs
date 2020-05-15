using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblFleetActionTypes
    {
        public TblFleetActionTypes()
        {
            TblFleets = new HashSet<TblFleets>();
        }

        public int FleetActionTypeId { get; set; }
        public string FleetActionTypeName { get; set; }

        public virtual ICollection<TblFleets> TblFleets { get; set; }
    }
}
