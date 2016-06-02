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
        Task<IdentityResult> CreateUser(User user,string password);
        Task SendRegistrationEmailAsync(string userId, string callbackurl);

    }
}
