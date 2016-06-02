using System;
using System.ComponentModel.DataAnnotations;
using EntityModel.Contracts;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EntityModel.Models
{
    public class User:IdentityUser, IDisposable
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        public DateTime JoinDate { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
