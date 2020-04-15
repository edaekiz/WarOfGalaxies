using System;
using System.Collections.Generic;

namespace WarOfGalaxiesApi.DAL.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> Where(Func<T, bool> predicate);
        List<T> ToList();
        IEnumerable<T> All();
        T Find(int id);
        bool Any(Func<T, bool> predicate);
        T FirstOrDefault(Func<T, bool> predicate);
        T Update(T entity);
        void Delete(T entity);
        void Delete(int id);
        void Delete(IEnumerable<T> entities);
        T Add(T entity);
    }
}
