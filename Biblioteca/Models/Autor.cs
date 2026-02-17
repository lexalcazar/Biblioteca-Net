
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Autor
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = "";

        public List<Libro> Libros { get; set; } = new();
    }
}
