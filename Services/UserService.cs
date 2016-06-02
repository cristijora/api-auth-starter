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

        public async Task<IdentityResult> CreateUser(User user,string password)
        {
            IdentityResult addUserResult = await _applicationUserManager.CreateAsync(user, password);
            if (addUserResult == null)
            {
                throw new UserNotCreatedException();
            }
            string code = await _applicationUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            return addUserResult;
        }

        public async Task SendRegistrationEmailAsync(string userId,string callbackUrl)
        {
            //TODO save email template somehwere
           await  _applicationUserManager.SendEmailAsync(userId, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        }
    }
}
