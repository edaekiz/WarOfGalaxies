using System;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;

namespace WarOfGalaxiesApi.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private db_warofgalaxyContext _context;
        public UnitOfWork()
        {
            _context = new db_warofgalaxyContext();
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        public int SaveChanges()
        {
            // Değişiklikleri kayıt ediyoruz.
            return _context.SaveChanges();
        }

        #region Disposable

        public bool Disposed { get; set; }
        public void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            this.Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
