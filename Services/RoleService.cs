using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModel.Models;
using Microsoft.AspNet.Identity;
using Repository.Utils;
using Services.Contracts;

namespace Services
{
    public class RoleService:IRoleService
    {
        private ApplicationRoleManager _applicationRoleManager;
        public RoleService(ApplicationRoleManager applicationRoleManager)
        {
            _applicationRoleManager = applicationRoleManager;
        }
        public async Task<Role> FindByIdAsync(string id)
        {
            return await _applicationRoleManager.FindByIdAsync(id);
        }

        public async Task<IdentityResult> CreateAsync(Role role)
        {
            return await _applicationRoleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> DeleteAsync(Role role)
        {
            return await _applicationRoleManager.DeleteAsync(role);
        }

        public IQueryable<Role> GetAll()
        {
            return _applicationRoleManager.Roles;
        }
    }
}
