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
            foreach (var r in roles)
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));

            // 2) Usuarios seed
            await CrearUsuarioSiNoExiste(userManager,
                email: "biblio@local.com",
                password: "Biblio123!",
                rol: "Bibliotecario");

            await CrearUsuarioSiNoExiste(userManager,
                email: "usuario@local.com",
                password: "Usuario123!",
                rol: "Usuario");
        }

        private static async Task CrearUsuarioSiNoExiste(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string rol)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    // Si falla por política de contraseña, cambia password y vuelve a ejecutar.
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(user, rol))
                await userManager.AddToRoleAsync(user, rol);
        }
    }
}

