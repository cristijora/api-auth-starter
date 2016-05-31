using DataAcces.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataAcces.Context
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext() : base("DataContext", throwIfV1Schema: false)
        {
            
        }

        public static DataContext Create()
        {
            return new DataContext();
        }
    }
}