using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblBuildingLevels
    {
        public int BuildingId { get; set; }
        public int BuildingLevel { get; set; }
        public long UpgradeTime { get; set; }
        public int BuildingValue { get; set; }
        public int RequiredMetal { get; set; }
        public int RequiredCrystal { get; set; }
        public int RequiredBoron { get; set; }
        public int RequiredEnergy { get; set; }
    }
}
