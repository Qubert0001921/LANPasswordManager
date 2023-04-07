using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Domain.Repositories;

public interface IBaseRepository<TEntity, TID>
{
    Task<TEntity> GetByIdAsync(TID id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task CreateOneAsync(TEntity entity);
    Task UpdateOneAsync(TEntity entity);
    Task RemoveOneByIdAsync(TID id);
    Task RemoveRangeByIdsAsync(IEnumerable<TID> ids);
}
