using BusinessParkCompaniesManagement.Server.Models.Entities;
using BusinessParkCompaniesManagement.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessParkCompaniesManagement.Server.Data
{
    public static class DbInitializer
    {
        public static void Initialize(
            AppDbContext context,
            IPasswordService passwordService,
            string seedAdminPassword)
        {
            context.Database.Migrate();

            if (!context.AdminUsers.Any())
            {
                var (hash, salt) = passwordService.HashPassword(seedAdminPassword);

                context.AdminUsers.AddRange(
                    new AdminUser
                    {
                        Username = "admin1",
                        FullName = "System Administrator 1",
                        Email = "admin1@businesspark.local",
                        PasswordHash = hash,
                        PasswordSalt = salt
                    },
                    new AdminUser
                    {
                        Username = "admin2",
                        FullName = "System Administrator 2",
                        Email = "admin2@businesspark.local",
                        PasswordHash = hash,
                        PasswordSalt = salt
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
