using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;

namespace WarOfGalaxiesApi.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private db_warofgalaxyContext _dbContext;

        public GenericRepository(db_warofgalaxyContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Verillen entityi veritabanına ekliyoruz.
        /// </summary>
        /// <param name="entity">Veritabanına eklenecek olan entity.</param>
        /// <returns></returns>
        public T Add(T entity)
        {
            return _dbContext.Add(entity).Entity;
        }

        /// <summary>
        /// Koşulu verilen dataları döner.
        /// </summary>
        /// <param name="predicate">Getirilecek olan kayıtların sorgusu.</param>
        /// <returns></returns>
        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        /// <summary>
        /// Koşulu verilen dataları döner.
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            return _dbContext.Set<T>().ToList();
        }

        /// <summary>
        /// Verilen entityi veritabanından siler.
        /// </summary>
        /// <param name="entity">Silinecek olan entity.</param>
        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Primary key idsi verilen entityi siler.
        /// </summary>
        /// <param name="id">Silinecek olan entitynin primary keyi.</param>
        public void Delete(int id)
        {
            // Entityi buluyoruz.
            T entity = _dbContext.Set<T>().Find(id);

            // Eğer bulamadıysak geri dönüyoruz.
            if (entity == null)
                return;

            // Silme işlemini yapyoruz.
            _dbContext.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Verlien entity listesini veritabanından siler.
        /// </summary>
        /// <param name="entities">Silinecek olan entityler.</param>
        public void Delete(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        /// <summary>
        /// Primary key idsi verilen entityi döner.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Find(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        /// <summary>
        /// Koşulları sağlayan ilk kaydı döner.
        /// </summary>
        /// <param name="predicate">Dönülecek olan koşul.</param>
        /// <returns></returns>
        public T FirstOrDefault(Func<T, bool> predicate)
        {
            return _dbContext.Set<T>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Entityi günceller. Entity default olarak tracking modunda olduğu için update gerekmeyecektir.
        /// </summary>
        /// <param name="entity">Güncellenecek olan entity.</param>
        /// <returns>Güncellenmiş halini döner.</returns>
        public T Update(T entity)
        {
            EntityEntry<T> updatedItem = _dbContext.Set<T>().Update(entity);
            return updatedItem.Entity;
        }
    }
}
