using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Authorization;
using Biblioteca.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Controllers
{
    // Solo el bibliotecario puede gestionar libros
    
    public class LibroesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibroesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Libroes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Libros.ToListAsync());
        }

        // GET: Libroes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var libro = await _context.Libros
                .Include(l => l.Autores)   // <-- clave
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null) return NotFound();

            return View(libro);
        }
        [Authorize(Roles = "Bibliotecario")]

        // GET: Libroes/Create
        public IActionResult Create()
        {

            var vm = new LibroCreateVm
            {
                Autores = _context.Autores
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Nombre
                    })
                    .ToList()
                };
            return View(vm); 
        }

        // POST: Libroes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LibroCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Autores = _context.Autores
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Nombre
                    })
                    .ToList();

                return View(vm);
            }

            var libro = new Libro
            {
                Titulo = vm.Titulo,
                Isbn = vm.Isbn,
                Copias = vm.Copias,
                Autores = _context.Autores
                    .Where(a => vm.AutoresSeleccionados.Contains(a.Id))
                    .ToList()
            };

            _context.Add(libro);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Libroes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null) return NotFound();

            return View(libro);
        }

        // POST: Libroes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Isbn,Copias")] Libro libro)
        {
            if (id != libro.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(libro);

            try
            {
                _context.Update(libro);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Libros.Any(e => e.Id == libro.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Libroes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var libro = await _context.Libros
                .FirstOrDefaultAsync(m => m.Id == id);

            if (libro == null) return NotFound();

            return View(libro);
        }

        // POST: Libroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

