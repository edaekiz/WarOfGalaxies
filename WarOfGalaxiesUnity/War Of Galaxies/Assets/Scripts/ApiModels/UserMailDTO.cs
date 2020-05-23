using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class UserMailDTO
    {
        public int UserMailId;
        public int MailCategoryId;
        public string MailContent;
        public string MailDate;
        public bool IsReaded;
    }

    [Serializable]
    public class LatestUnReadedUserMailRequestDTO
    {
        public int LastUnReadedMailId;
    }

    [Serializable]
    public class MailReadRequestDTO
    {
        public int UserMailId;
    }
}
