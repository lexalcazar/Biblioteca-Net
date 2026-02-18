using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models.ViewModels
{
    public class PrestamoCreateVm
    {
        [Required]
        public int LibroId { get; set; }

        [Required]
        [EmailAddress]
        public string EmailUsuario { get; set; } = "";

        public DateTime FechaInicio { get; set; } = DateTime.Today;

        public DateTime FechaFin { get; set; } = DateTime.Today.AddDays(15);

        // Lista de libros para el dropdown
        public List<SelectListItem> Libros { get; set; } = new();
    }
}
