using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUserMailCategories
    {
        public TblUserMailCategories()
        {
            TblUserMails = new HashSet<TblUserMails>();
        }

        public int UserMailCategoryId { get; set; }
        public string UserMailCategoryName { get; set; }

        public virtual ICollection<TblUserMails> TblUserMails { get; set; }
    }
}
