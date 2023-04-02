using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Domain.Repositories;

public interface IRoleRepository : IBaseRepository<Role, Guid>
{
    
}
