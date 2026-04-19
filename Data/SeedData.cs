using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Practica2_Teoria.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var db = services.GetRequiredService<ApplicationDbContext>();

        const string roleName = "Coordinador";
        const string coordinatorEmail = "coordinador@universidad.edu";
        const string coordinatorPassword = "Coord123!";

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var coordinator = await userManager.FindByEmailAsync(coordinatorEmail);
        if (coordinator == null)
        {
            coordinator = new IdentityUser
            {
                UserName = coordinatorEmail,
                Email = coordinatorEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(coordinator, coordinatorPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo crear el usuario coordinador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(coordinator, roleName))
        {
            await userManager.AddToRoleAsync(coordinator, roleName);
        }

        if (!await db.Cursos.AnyAsync())
        {
            db.Cursos.AddRange(
                new Models.Curso
                {
                    Codigo = "MAT101",
                    Nombre = "Matemáticas Básicas",
                    Creditos = 4,
                    CupoMaximo = 40,
                    HorarioInicio = new TimeOnly(8, 0),
                    HorarioFin = new TimeOnly(10, 0),
                    Activo = true
                },
                new Models.Curso
                {
                    Codigo = "FIS201",
                    Nombre = "Física I",
                    Creditos = 3,
                    CupoMaximo = 35,
                    HorarioInicio = new TimeOnly(10, 30),
                    HorarioFin = new TimeOnly(12, 0),
                    Activo = true
                },
                new Models.Curso
                {
                    Codigo = "PRO301",
                    Nombre = "Programación Avanzada",
                    Creditos = 5,
                    CupoMaximo = 30,
                    HorarioInicio = new TimeOnly(14, 0),
                    HorarioFin = new TimeOnly(16, 30),
                    Activo = true
                });

            await db.SaveChangesAsync();
        }
    }
}
