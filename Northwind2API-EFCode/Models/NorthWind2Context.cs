using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Northwind2API_EFCode.Models
{
    public class NorthWind2Context : DbContext
    {
        public DbSet<Address> Address { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }

        public NorthWind2Context(DbContextOptions<NorthWind2Context> options) : base(options) 
        {
        }

    }
}
