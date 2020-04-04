using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarOfGalaxiesApi.Repositories;

namespace WarOfGalaxiesApi.Interfaces
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
        int SaveChanges();
    }
}
