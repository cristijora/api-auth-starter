using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModel.Models;
using Microsoft.AspNet.Identity;

namespace Services.Contracts
{
    public interface IRoleService
    {
        Task<Role> FindByIdAsync(string id);
        Task<IdentityResult> CreateAsync(Role role);
        Task<IdentityResult> DeleteAsync(Role role);
        IQueryable<Role> GetAll();
    }
}
