using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUsers
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public Guid UserToken { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreateDate { get; set; }
        public string GoogleToken { get; set; }
        public string IosToken { get; set; }
        public string UserLanguage { get; set; }
    }
}
