using System.ComponentModel.DataAnnotations;
namespace Biblioteca.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = "";

        public string? Isbn { get; set; }

        public List<Autor> Autores { get; set; } = new();
        public int Copias { get; set; } = 1;

    }
}
