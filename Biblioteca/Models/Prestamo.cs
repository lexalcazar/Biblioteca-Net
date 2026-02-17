using System.ComponentModel.DataAnnotations;
namespace Biblioteca.Models
{
    public class Prestamo
    {
        public int Id { get; set; }
        public int LibroId { get; set; }
        public Libro? Libro { get; set; }
        // Usuario de Identity (AspNetUsers.Id)
        [Required]
        public string UserId { get; set; } = "";

        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime FechaFin { get; set; } = DateTime.UtcNow.AddDays(15);

        public int Renovaciones { get; set; } = 0;
        public bool Devuelto { get; set; } = false;
    }
}
