using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            // 1) Roles
            string[] roles = { "Usuario", "Bibliotecario" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2) Usuario bibliotecario por defecto
            var adminEmail = "biblio@local.com";
            var adminPassword = "Biblio123!"; // cumple mayúscula/minúscula/número/símbolo

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    // Si falla por política de password, revisa los errores en result.Errors
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Bibliotecario"))
                await userManager.AddToRoleAsync(adminUser, "Bibliotecario");
        }
    }
}

