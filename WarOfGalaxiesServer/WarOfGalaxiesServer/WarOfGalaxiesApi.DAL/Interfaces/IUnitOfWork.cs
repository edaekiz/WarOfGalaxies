namespace WarOfGalaxiesApi.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        bool Disposed { get; set; }
        IGenericRepository<T> GetRepository<T>() where T : class;
        int SaveChanges();
    }
}
