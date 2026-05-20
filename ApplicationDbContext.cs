using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using E_Commerce_System.Models;

namespace E_Commerce_System
{
    internal class ApplicationDbContext : DbContext
    {
        internal object user;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //connection t database
            options.UseSqlServer(" Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=E-CommerceSystem; Integrated Security=true; TrustServerCertificate=True ");
        }

        // register the models 

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Review> Reviews { get; set; }

    }
}
