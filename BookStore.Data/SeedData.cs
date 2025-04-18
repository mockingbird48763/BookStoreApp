using Azure.Core;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Roles
            var admin = new Role { Name = "Admin" };

            var user = new Role { Name = "User" };
            context.Roles.AddRange(admin, user);
            await context.SaveChangesAsync();

            // Members
            context.Members.AddRange(
                    new Member
                    {
                        Email = "admin@example.com",
                        Password = "$2a$11$VLzTLJL/pl7VDrEOJrS3quKL1zpRqtW5vSAwP46I/ILdsDPNLQU4m",
                        Roles = [admin]
                    },
                    new Member
                    {
                        Email = "user@example.com",
                        Password = "$2a$11$VLzTLJL/pl7VDrEOJrS3quKL1zpRqtW5vSAwP46I/ILdsDPNLQU4m",
                        Roles = [user]
                    },
                    new Member
                    {
                        Email = "super@example.com",
                        Password = "$2a$11$VLzTLJL/pl7VDrEOJrS3quKL1zpRqtW5vSAwP46I/ILdsDPNLQU4m",
                        Roles = [admin, user]
                    }
            );
            await context.SaveChangesAsync();
        }
    }
}
