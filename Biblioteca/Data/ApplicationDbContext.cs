using Biblioteca.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Autor> Autores => Set<Autor>();
        public DbSet<Libro> Libros => Set<Libro>();
        public DbSet<Prestamo> Prestamos => Set<Prestamo>();
    }
}
