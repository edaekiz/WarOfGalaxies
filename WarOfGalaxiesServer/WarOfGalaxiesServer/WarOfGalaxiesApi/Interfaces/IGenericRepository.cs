using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WarOfGalaxiesApi.Interfaces
{
    public interface IGenericRepository<T, TID> where T : class where TID : struct
    {
        void Insert(T ent);
        void Update(T ent);
    }
}
