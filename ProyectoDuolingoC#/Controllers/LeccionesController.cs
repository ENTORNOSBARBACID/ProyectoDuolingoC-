using Microsoft.AspNetCore.Mvc;
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
            string leccion = await this.repo.VerContenido(id);
            return View("Index", leccion);
        }
    }

}
