using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteca.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles="Bibliotecario")] 
        public IActionResult Bienvenida()
        {
            return View();
        }
        [Authorize(Roles ="Usuario")]
        public IActionResult IndexLog()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privado()
        {
            return Content("OK: estás autenticado");
        }
        [Authorize(Roles = "Bibliotecario")]
        public IActionResult SoloBiblio()
        {
            return Content("OK: eres bibliotecario");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
