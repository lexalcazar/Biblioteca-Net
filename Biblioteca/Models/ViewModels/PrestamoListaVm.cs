using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models.ViewModels
{
    public class PrestamoListaVm
    {
        public int Id { get; set; }
        public string LibroTitulo { get; set; } = "";
        public string EmailUsuario { get; set; } = "";
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Renovaciones { get; set; }
        public bool Devuelto { get; set; }
    }
}
