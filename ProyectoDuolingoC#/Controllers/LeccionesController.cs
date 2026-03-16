using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoDuolingoC_.Models;
using ProyectoDuolingoC_.Repositories;
using System.Threading.Tasks;

namespace ProyectoDuolingoC_.Controllers
{
    public class LeccionesController : Controller
    {
        RepositoryLecciones repo;
        RepositoryCursos cursos;
        public LeccionesController(RepositoryLecciones repo, RepositoryCursos cursos)
        {
            this.repo = repo;
            this.cursos = cursos;
        }
        public async Task<IActionResult> Index(int id)
        {
            Leccion leccion = await this.repo.VerContenido(id);
            return View("Index", leccion);
        }
        [Authorize(Policy = "SOLOADMIN")]
        public async Task<IActionResult> Create(int id)
        {
            ViewData["CURSOID"]= id;
            var cur = await cursos.FindCurso(id);
            ViewData["NOMBRECURSO"] = cur.Titulo;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Leccion lec)
        {
            await this.repo.CreateLeccionAsync(lec);
            return RedirectToAction("Details", "Cursos", new { id = lec.CursoID});
        }
    }

}
