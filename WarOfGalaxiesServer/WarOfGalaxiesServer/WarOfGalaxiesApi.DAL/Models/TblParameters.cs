using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblParameters
    {
        public int ParameterId { get; set; }
        public string Description { get; set; }
        public int? ParameterIntValue { get; set; }
        public double? ParameterFloatValue { get; set; }
        public DateTime? ParameterDateTimeValue { get; set; }
        public bool? ParameterBitValue { get; set; }
        public bool ParameterSendToUserValue { get; set; }
    }
}
