using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flexigroup.BSO.Transfer.Models
{
    public class BSOContext : DbContext
    {
        public DbSet<BsoData> BsoDatas { get; set; }
        private string connectionString;

        public BSOContext(string conn)
        {
            this.connectionString = conn;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Data Source=13.70.87.41;Database=Flexigroup;User ID=flexigroup_user;Password=Flex!5000");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
