using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Models;

namespace BookStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Role> Roles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Email)
                .IsUnique();

            // modelBuilder.Entity<Role>().HasData(
            //    new Role { Id = 1, Name = "Admin" },
            //    new Role { Id = 2, Name = "User" }
            // );
        }
    }
}
