using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using UserList.Domain.Entities;

namespace UserList.API.Data
{
    public class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            // Получение контекста БД
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                // Выполнение миграций
                await context.Database.MigrateAsync();
            }

            if(!context.Roles.Any())
            {
                var _roles = context.Set<Role>();

                await _roles.AddAsync(new Role { Id = 1, Name = "User"});
                await _roles.AddAsync(new Role { Id = 2, Name = "Admin" });
                await _roles.AddAsync(new Role { Id = 3, Name = "Support" });
                await _roles.AddAsync(new Role { Id = 4, Name = "SuperAdmin" });

                context.SaveChanges();
            }

        }
    }
}
