using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Models.ViewModels
{
    public class LibroEditVm
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Isbn { get; set; }
        public int Copias { get; set; }

        // autores seleccionados
        public List<int> AutoresSeleccionados { get; set; } = new();

        // lista de autores
        public List<SelectListItem> Autores { get; set; } = new();
    }
}
