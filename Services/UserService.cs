using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;
using EntityModel.Models;
using Microsoft.AspNet.Identity;
using Repository.Utils;
using Services.Contracts;

namespace Services
{
    public class UserService:IUserService
    {
        private ApplicationUserManager _applicationUserManager;
        public UserService(ApplicationUserManager applicationUserManager)
        {
            _applicationUserManager = applicationUserManager;
        }
        public IEnumerable<User> GetAll()
        {
            return _applicationUserManager.Users;
        }

        public async Task<User> GetById(string Id)
        {
            var user = await _applicationUserManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new UserNotFoundException();    
            }
            return user;
        }

        public async Task<User> GetUserByName(string username)
        {
            var user = await _applicationUserManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            return user;
        }

        public async Task<IdentityResult> DeleteAsync(User user)
        {
            return await _applicationUserManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> CreateUser(User user,string password)
        {
            IdentityResult addUserResult = await _applicationUserManager.CreateAsync(user, password);
            if (addUserResult == null)
            {
                throw new UserNotCreatedException();
            }
            return addUserResult;
        }

        public async Task SendRegistrationEmailAsync(string userId,Uri callbackUrl)
        {
            //TODO save email template somehwere
           await  _applicationUserManager.SendEmailAsync(userId, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
        {
            return await _applicationUserManager.GenerateEmailConfirmationTokenAsync(userId);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string code)
        {
            return await _applicationUserManager.ConfirmEmailAsync(userId, code);
        }

        public bool IsInRole(string userId, string roleName)
        {
            return _applicationUserManager.IsInRole(userId, roleName);
        }

        public async Task<IdentityResult> AddToRoleAsync(string userId, string roleName)
        {
            return await _applicationUserManager.AddToRoleAsync(userId, roleName);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(string userId, string roleName)
        {
            return await _applicationUserManager.RemoveFromRoleAsync(userId, roleName);
        }
    }
}
