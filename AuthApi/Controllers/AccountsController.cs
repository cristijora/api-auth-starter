﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DtoModels;
using EntityModel.Models;
using Microsoft.AspNet.Identity;
using Services;
using Services.Contracts;

namespace AuthApi.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        private IUserService userService;

        public AccountsController()
        {
            userService=new UserService(this.AppUserManager);
        }
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(userService.GetAll().ToList().Select(u => this.TheModelFactory.Create(u)));
        }
        
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await userService.GetById(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await userService.GetUserByName(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                JoinDate = DateTime.Now.Date,
            };

            IdentityResult addUserResult = await userService.CreateUser(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            var callbackUrl = new Uri(System.Security.Policy.Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));

            await userService.SendRegistrationEmailAsync(user.Id, callbackUrl);

            Uri locationHeader = new Uri(System.Security.Policy.Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));

        }
    }
}
