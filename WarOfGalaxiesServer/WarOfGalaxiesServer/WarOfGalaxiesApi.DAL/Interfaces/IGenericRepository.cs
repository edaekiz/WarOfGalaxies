using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WarOfGalaxiesApi.DAL.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
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
        EntityState GetStateOfEntry(T entity);
        int Count(Expression<Func<T, bool>> predicate);
    }
}
