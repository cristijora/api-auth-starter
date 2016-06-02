using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModel.Models;

namespace DataAcces.Context
{
    public class DataBaseModelDefinitions
    {
        public void DefineConfigurations(DbModelBuilder modelBuilder) 
        {
            //db definitions here
            DefineTableNames(modelBuilder);
        }

        private void DefineTableNames(DbModelBuilder modelBuilder)
        {
            //TODO custom table names here
             modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}
