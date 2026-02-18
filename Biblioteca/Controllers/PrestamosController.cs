using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Models.ViewModels;
using System.Linq;



namespace Biblioteca.Controllers
{
    [Authorize(Roles = "Usuario,Bibliotecario")]
    public class PrestamosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PrestamosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Usuario: ve sus préstamos (activos)
        public async Task<IActionResult> MisPrestamos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var prestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Where(p => p.UserId == userId && !p.Devuelto)
                .ToListAsync();
            return View(prestamos);
        }

        // ----------- CREAR PRÉSTAMO (SOLO BIBLIOTECARIO) -----------
        public async Task<IActionResult> Crear()
        {
            var vm = new PrestamoCreateVm
            {
                Libros = await _context.Libros
                    .OrderBy(l => l.Titulo)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.Titulo
                    })
                    .ToListAsync()
            };
            return View(vm);
        }


        [Authorize(Roles = "Bibliotecario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(PrestamoCreateVm vm)
        {
            // Recargar dropdown si hay errores
            async Task RecargarLibros()
            {
                vm.Libros = await _context.Libros
                    .OrderBy(l => l.Titulo)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.Titulo
                    })
                    .ToListAsync();
            }

            if (!ModelState.IsValid)
            {
                await RecargarLibros();
                return View(vm);
            }

            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == vm.LibroId);
            if (libro == null)
            {
                ModelState.AddModelError("", "El libro no existe.");
                await RecargarLibros();
                return View(vm);
            }

            // Buscar usuario por email
            var usuario = await _userManager.FindByEmailAsync(vm.EmailUsuario);
            if (usuario == null)
            {
                ModelState.AddModelError("", "No existe ningún usuario con ese email.");
                await RecargarLibros();
                return View(vm);
            }

            // Comprobar copias disponibles
            var prestamosActivos = await _context.Prestamos
                .CountAsync(p => p.LibroId == vm.LibroId && !p.Devuelto);

            if (prestamosActivos >= libro.Copias)
            {
                ModelState.AddModelError("", "No hay copias disponibles de este libro.");
                await RecargarLibros();
                return View(vm);
            }

            // Crear préstamo
            var prestamo = new Prestamo
            {
                LibroId = vm.LibroId,
                UserId = usuario.Id,
                FechaInicio = vm.FechaInicio,
                FechaFin = vm.FechaFin,
                Renovaciones = 0,
                Devuelto = false
            };

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            TempData["ok"] = "Préstamo creado correctamente.";
            return RedirectToAction(nameof(Crear));
        }
    }    
}

