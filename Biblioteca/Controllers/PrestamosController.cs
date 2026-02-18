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
using Biblioteca.Models.ViewModels;



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

        // Usuario: ve su historial completo de préstamos
        public async Task<IActionResult> HistorialPrestamos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var prestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.FechaInicio)
                .ToListAsync();
            return View(prestamos);
        }

        //--- VER TODOS LOS PRESTAMOS SOLO BIBLIOTECARIO ---
        [Authorize(Roles = "Bibliotecario")]
        public async Task<IActionResult> Index()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .OrderByDescending(p => p.FechaInicio)
                .ToListAsync();

            // Traer emails de usuarios (rápido y sin liarnos con joins raros)
            var userIds = prestamos.Select(p => p.UserId).Distinct().ToList();
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            var emailPorId = users.ToDictionary(u => u.Id, u => u.Email ?? "");

            var vm = prestamos.Select(p => new PrestamoListaVm
            {
                Id = p.Id,
                LibroTitulo = p.Libro != null ? p.Libro.Titulo : "",
                EmailUsuario = emailPorId.ContainsKey(p.UserId) ? emailPorId[p.UserId] : p.UserId,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                Renovaciones = p.Renovaciones,
                Devuelto = p.Devuelto
            }).ToList();

            return View(vm);
        }

      
        //------- DEVOLVER PRÉSTAMO (BIBLIOTECARIO) --------
        [Authorize(Roles = "Bibliotecario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Devolver(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (prestamo == null) return NotFound();

            prestamo.Devuelto = true;

            // Aumentar copias al devolver
            if (prestamo.Libro != null)
            {
                prestamo.Libro.Copias++;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //------- RENOVAR PRÉSTAMO (USUARIO) --------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renovar(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var prestamo = await _context.Prestamos
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.Devuelto);

            if (prestamo == null)
            {
                TempData["error"] = "No se pudo encontrar el préstamo o ya fue devuelto.";
                return RedirectToAction(nameof(MisPrestamos));
            }

            // Extender fecha fin por 15 días
            prestamo.FechaFin = prestamo.FechaFin.AddDays(15);
            prestamo.Renovaciones++;

            await _context.SaveChangesAsync();

            TempData["ok"] = "Préstamo renovado por 15 días más.";
            return RedirectToAction(nameof(MisPrestamos));
        }

        //------- RENOVAR PRÉSTAMO (BIBLIOTECARIO) --------
        [Authorize(Roles = "Bibliotecario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenovarBibliotecario(int id)
        {
            var prestamo = await _context.Prestamos
                .FirstOrDefaultAsync(p => p.Id == id && !p.Devuelto);

            if (prestamo == null)
            {
                TempData["error"] = "No se pudo encontrar el préstamo o ya fue devuelto.";
                return RedirectToAction(nameof(Index));
            }

            // Extender fecha fin por 15 días
            prestamo.FechaFin = prestamo.FechaFin.AddDays(15);
            prestamo.Renovaciones++;

            await _context.SaveChangesAsync();

            TempData["ok"] = "Préstamo renovado por 15 días más.";
            return RedirectToAction(nameof(Index));
        }

        // ----------- CREAR PRÉSTAMO (SOLO BIBLIOTECARIO) -----------
        [Authorize(Roles = "Bibliotecario")]
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
            if (libro.Copias <= 0)
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

            // Actualizar copias: decrementar al prestar
            libro.Copias--;

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            TempData["ok"] = "Préstamo creado correctamente.";
            return RedirectToAction(nameof(Crear));
        }
    }    
}

