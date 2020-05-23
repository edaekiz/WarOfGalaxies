using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class TblUsers
    {
        public TblUsers()
        {
            TblUserMails = new HashSet<TblUserMails>();
            TblUserPlanets = new HashSet<TblUserPlanets>();
            TblUserResearchUpgs = new HashSet<TblUserResearchUpgs>();
            TblUserResearches = new HashSet<TblUserResearches>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public Guid UserToken { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreateDate { get; set; }
        public string GoogleToken { get; set; }
        public string IosToken { get; set; }
        public string UserLanguage { get; set; }

        public virtual ICollection<TblUserMails> TblUserMails { get; set; }
        public virtual ICollection<TblUserPlanets> TblUserPlanets { get; set; }
        public virtual ICollection<TblUserResearchUpgs> TblUserResearchUpgs { get; set; }
        public virtual ICollection<TblUserResearches> TblUserResearches { get; set; }
    }
}
