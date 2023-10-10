using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UserList.Domain.Entities;

namespace UserList.API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
