using Backend.Src.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Src.Seeder;

public static class Seeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("⚠️  Usuarios ya existen, omitiendo seeding.");
            return;
        }   
        var adminRole = new Role { Name = "Administrador" };
        var clientRole = new Role { Name = "Cliente" };

        await context.Roles.AddRangeAsync(adminRole, clientRole);
        
        await context.SaveChangesAsync();
        var users = new User
        {
            Name = "Admin",
            LastName = "Teatro",
            Email = "admin@divine.cl",
            Rut = "12345678K",
            Phone = "912345678",
            Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Status = true,
            RoleId = adminRole.Id
        };
        context.Users.Add(users);
        await context.SaveChangesAsync();
    }
}