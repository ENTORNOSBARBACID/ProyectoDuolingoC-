using Microsoft.AspNetCore.Mvc;
using ProyectoDuolingoC_.Models;
using ProyectoDuolingoC_.Repositories;
using System.Threading.Tasks;

namespace ProyectoDuolingoC_.Controllers
{
    public class LeccionesController : Controller
    {
        RepositoryLecciones repo;
        public LeccionesController(RepositoryLecciones repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index(int id)
        {
            Leccion leccion = await this.repo.VerContenido(id);
            return View("Index", leccion);
        }
        public IActionResult Create(int id)
        {
            ViewData["CURSOID"]= id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Leccion lec)
        {
            await this.repo.CreateLeccionAsync(lec);
            return RedirectToAction("Details", "Cursos");
        }
    }

}
