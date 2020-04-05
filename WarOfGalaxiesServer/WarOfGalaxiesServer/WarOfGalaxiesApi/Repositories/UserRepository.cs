using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarOfGalaxiesApi.Interfaces;
using WarOfGalaxiesApi.DAL.Models;

namespace WarOfGalaxiesApi.Repositories
{
    public class UserRepository : IGenericRepository<TblUsers, long>
    {
        public void Insert(TblUsers ent)
        {
        }

        public void Update(TblUsers ent)
        {
        }
    }
}
