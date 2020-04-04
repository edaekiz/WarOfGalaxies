using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DTO.Models
{
    [Serializable]
    public partial class UserDTO
    {
        public int UserId;
        public string Username;
        public Guid UserToken;
        public bool IsBanned;
        public DateTime CreateDate;
        public string GoogleToken;
        public string IosToken;
        public string UserLanguage;
    }
}
