using Microsoft.AspNetCore.Mvc;
using ProyectoDuolingoC_.Models;
using ProyectoDuolingoC_.Repositories;
using System.Threading.Tasks;

namespace ProyectoDuolingoC_.Controllers
{
    public class CursosController : Controller
    {
        RepositoryCursos repo;
        RepositoryLecciones repoLec;
        public CursosController(RepositoryCursos repo, RepositoryLecciones repoLec) 
        {
            this.repo = repo;
            this.repoLec = repoLec;
        }
        public async Task<IActionResult> Details(int id)
        {
            Curso curso = await this.repo.FindCurso(id);

            if (curso == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Leccion> lec = await this.repoLec.LoadLecciones(id);
            ViewData["LECCIONES"] = lec;

            int? idUsu = HttpContext.Session.GetInt32("ID");

            if (idUsu != null)
            {
                CursosUsuario cursosUsuario = await this.repo.VerCursousuarioAsync(id, idUsu.Value);

                ViewData["CURSO"] = cursosUsuario == null;
            }
            else
            {
                ViewData["CURSO"] = true;
            }

            return View(curso);
        }
        public async Task<IActionResult> Inscribirse(int id)
        {
            int? idUsu = HttpContext.Session.GetInt32("ID");

            if (idUsu == null)
            {
                TempData["Titulo"] = "¡Ups!";
                TempData["Mensaje"] = "Debes iniciar sesión para poder inscribirte en este curso.";
                TempData["Icono"] = "warning";
                return RedirectToAction("Login", "Home");
            }

            
            await this.repo.Inscribirse(id, idUsu.Value);
            
            TempData["Titulo"] = "¡Genial!";
            TempData["Mensaje"] = "Te has inscrito al curso correctamente.";
            TempData["Icono"] = "success"; // Esto mostrará el check verde


            return RedirectToAction("Details", new { id = id });
        }

    }
}
