using System;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class UserDTO
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
