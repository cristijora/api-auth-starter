using DataAcces.Context;
using EntityModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataAcces.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DataAcces.Context.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DataAcces.Context.DataContext context)
        {
            var manager = new UserManager<User>(new UserStore<User>(new DataContext()));

            var user = new User
            {
                UserName = "admin",
                Email = "joracristi@gmail.com",
                EmailConfirmed = true,
                FirstName = "Cristi",
                LastName = "Jora",
                JoinDate = DateTime.Now.AddYears(-3)
            };

            manager.Create(user, "admintest");
        }
    }
}
