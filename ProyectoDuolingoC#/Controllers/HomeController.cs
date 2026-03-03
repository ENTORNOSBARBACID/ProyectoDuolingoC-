using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProyectoDuolingoC_.Models;
using ProyectoDuolingoC_.Repositories;

namespace ProyectoDuolingoC_.Controllers
{
    public class HomeController : Controller
    {

        RepositoryLogIn repo;
        RepositoryCursos repoCursos;
        public HomeController(RepositoryLogIn repo, RepositoryCursos repoCursos)
        {
            this.repo = repo;
            this.repoCursos = repoCursos;
        }

        public async Task<IActionResult> Index()
        {
            List<Curso> cur = await this.repoCursos.LoadCursos();
            return View(cur);
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Usuario user, string pass)
        {
            await this.repo.RegisterUsuario(user.NombreUsuario, user.CorreoElectronico, user.Imagen, user.Rol, pass);
            ViewData["MENSAJE"] = "Usuario en el sistema";
            return View();
        }
        public async Task<IActionResult> LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string pass)
        {
            Usuario user = await this.repo.LogInUserAsync(email, pass);
            if (user == null)
            {
                ViewData["MENSAJE"] = "Creedenciales no validas";
            }
            else

            {
                HttpContext.Session.SetInt32("ID", user.UsuarioID);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.Remove("ID");
            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
