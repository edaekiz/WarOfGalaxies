using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public partial class UserDTO
    {
        public int UserId;
        public string Username;
        public string UserToken;
        public bool IsBanned;
        public DateTime CreateDate;
        public string GoogleToken;
        public string IosToken;
        public string UserLanguage;
    }
}
