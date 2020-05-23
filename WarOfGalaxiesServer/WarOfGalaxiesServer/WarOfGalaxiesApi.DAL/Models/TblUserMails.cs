using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserMails
    {
        public int UserMailId { get; set; }
        public int UserId { get; set; }
        public int MailCategoryId { get; set; }
        public string MailContent { get; set; }
        public DateTime MailDate { get; set; }
        public bool IsReaded { get; set; }

        public virtual TblUserMailCategories MailCategory { get; set; }
        public virtual TblUsers User { get; set; }
    }
}
