using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Biblioteca.Controllers
{
    // Prestamos: crear, renovar, devolver, listar (solo los suyos)
    [Authorize(Roles = "Usuario,Bibliotecario")]
    public class PrestamosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrestamosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Usuario: ve solo los suyos
        // Bibliotecario: también puede ver los suyos (y si quieres luego hacemos "Todos")
        public IActionResult MisPrestamos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var prestamos = _context.Prestamos
                .Where(p => p.UserId == userId && !p.Devuelto)
                .ToList();

            return View(prestamos);
        }

        // Renovar: usuario solo los suyos, bibliotecario cualquiera
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Renovar(int id)
        {
            var prestamo = _context.Prestamos.FirstOrDefault(p => p.Id == id);

            if (prestamo == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.IsInRole("Bibliotecario") && prestamo.UserId != userId)
                return Forbid();

            prestamo.FechaFin = prestamo.FechaFin.AddDays(15);
            prestamo.Renovaciones++;

            _context.SaveChanges();

            return RedirectToAction(nameof(MisPrestamos));
        }

        // Crear préstamo: SOLO bibliotecario
        [Authorize(Roles = "Bibliotecario")]
        [HttpGet]
        public IActionResult Crear()
        {
            // luego metemos un formulario: seleccionar libro + usuario
            return View();
        }
    }
}
