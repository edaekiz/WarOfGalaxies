using System;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class UserMailDTO
    {
        public int UserMailId { get; set; }
        public int MailCategoryId { get; set; }
        public string MailContent { get; set; }
        public DateTime MailDate { get; set; }
        public bool IsReaded { get; set; }
    }

    public class LatestUnReadedUserMailRequestDTO
    {
        public int LastUnReadedMailId { get; set; }
    }

    public class MailReadRequestDTO
    {
        public int UserMailId { get; set; }
    }
}
