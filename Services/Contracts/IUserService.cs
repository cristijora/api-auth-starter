using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModel.Models;
using Microsoft.AspNet.Identity;

namespace Services.Contracts
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        Task<User> GetById(string Id);
        Task<User> GetUserByName(string username);
        Task<IdentityResult> DeleteAsync(User user);
        Task<IdentityResult> CreateUser(User user,string password);
        Task SendRegistrationEmailAsync(string userId, Uri callbackurl);
        Task<string> GenerateEmailConfirmationTokenAsync(string userId);
        Task<IdentityResult> ConfirmEmailAsync(string userId,string code);
        bool IsInRole(string userId,string roleName);
        Task<IdentityResult> AddToRoleAsync(string userId,string roleName);
        Task<IdentityResult> RemoveFromRoleAsync(string userId, string roleName);

    }
}
