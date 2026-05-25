using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Seeder;

public static class DbSeeder
{
    public static async Task SeedAsync(WmsDbContext context)
    {
        if (!context.Roles.Any())
        {
            var roles = new List<Role>
            {
                new()
                {
                    RoleName = "Admin",
                    Description = "System Administrator"
                },

                new()
                {
                    RoleName = "Manager",
                    Description = "Department Manager"
                },

                new()
                {
                    RoleName = "Employee",
                    Description = "Regular Employee"
                }
            };

            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
        }

        if (!context.Departments.Any())
        {
            var departments = new List<Department>
            {
                new()
                {
                    DepartmentName = "Human Resources"
                },

                new()
                {
                    DepartmentName = "IT"
                },

                new()
                {
                    DepartmentName = "Finance"
                }
            };

            context.Departments.AddRange(departments);

            await context.SaveChangesAsync();
        }

        if (!context.UserLogins.Any())
        {
            var adminRole = context.Roles
                .First(r => r.RoleName == "Admin");

            var adminUser = new UserLogin
            {
                Username = "admin",

                PasswordHash = BCrypt.Net.BCrypt
                    .HashPassword("Admin@123"),

                RoleId = adminRole.RoleId
            };

            context.UserLogins.Add(adminUser);

            await context.SaveChangesAsync();
        }
    }
}
