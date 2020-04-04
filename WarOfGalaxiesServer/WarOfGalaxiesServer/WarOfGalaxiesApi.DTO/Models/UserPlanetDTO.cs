using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DTO.Models
{
    [Serializable]
    public partial class UserPlanetDTO
    {
        public int UserPlanetId;
        public int UserId;
        public int PlanetId;
        public DateTime CreateDate;
    }
}
