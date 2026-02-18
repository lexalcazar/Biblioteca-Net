using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Biblioteca.Controllers
{
    [Authorize(Roles = "Bibliotecario")]
    public class AdminController : Controller
    {   // Controlador para tareas administrativas, como crear usuarios
        private readonly UserManager<IdentityUser> _userManager;
        // Inyectamos UserManager para gestionar usuarios
        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        // Mostrar formulario para crear un nuevo usuario
        public IActionResult CrearUsuario()
        {
            return View(new CrearUsuarioVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Procesar el formulario de creación de usuario
        public async Task<IActionResult> CrearUsuario(CrearUsuarioVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Evitar duplicados
            var existente = await _userManager.FindByEmailAsync(vm.Email);
            if (existente != null)
            {
                ModelState.AddModelError("", "Ya existe un usuario con ese email.");
                return View(vm);
            }
            // Crear el nuevo usuario
            var user = new IdentityUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                EmailConfirmed = true
            };
            // Intentar crear el usuario con la contraseña proporcionada
            var result = await _userManager.CreateAsync(user, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);

                return View(vm);
            }

            // Rol por defecto: Usuario
            await _userManager.AddToRoleAsync(user, "Usuario");

            TempData["ok"] = "Usuario creado correctamente (rol: Usuario).";
            return RedirectToAction(nameof(CrearUsuario));
        }
        public IActionResult Usuarios()
        {
            var usuarios = _userManager.Users
                .Select(u => new UsuarioListaVm
                {
                    Id = u.Id,
                    Email = u.Email ?? "",
                    UserName = u.UserName ?? ""
                })
                .ToList();

            return View(usuarios);
        }
    }
    // ViewModel para la creación de usuarios
    public class CrearUsuarioVm
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
    
  

    public class UsuarioListaVm
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserName { get; set; } = "";
    }

}
