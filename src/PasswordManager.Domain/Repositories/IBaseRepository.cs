using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Repositories;

public interface IBaseRepository<TEntity, TID>
{
    Task<TEntity> GetById(TID id);
    IEnumerable<TEntity> GetAll();
    Task CreateOne(TEntity entity);
    Task UpdateOne(TEntity entity);
    Task RemoveOneById(TID id);
}
