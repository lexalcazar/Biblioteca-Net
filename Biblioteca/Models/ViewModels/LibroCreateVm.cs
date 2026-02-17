using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Models.ViewModels
{
    public class LibroCreateVm
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Isbn { get; set; }
        public int Copias { get; set; }

        // Autores seleccionados
        public List<int> AutoresSeleccionados { get; set; } = new();

        // Lista para el selector
        public List<SelectListItem> Autores { get; set; } = new();
    }
}
