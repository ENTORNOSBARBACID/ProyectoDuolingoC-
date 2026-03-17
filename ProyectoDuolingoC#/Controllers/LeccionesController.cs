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
        public async Task<IActionResult> Eliminar(int id, int idCurso)
        {
            await this.repo.EliminarLeccionEnCascada(id);
            TempData["Titulo"] = "¡Leccion eliminada!";
            TempData["Mensaje"] = "Se han borrado todos los datos asociados a esta leccion.";
            TempData["Icono"] = "success";
            return RedirectToAction("Details", "Cursos", new { id = idCurso });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Leccion leccion = await this.repo.VerContenido(id);

            // Necesitas buscar el curso para mandarle el nombre a la vista
            Curso curso = await this.cursos.FindCurso(leccion.CursoID);
            ViewData["NOMBRECURSO"] = curso.Titulo;

            return View(leccion);
        }
        [HttpPost]
        [Authorize(Policy = "SOLOADMIN")]
        public async Task<IActionResult> Edit(Leccion leccion)
        {
            ModelState.Remove("Curso");
            ModelState.Remove("Preguntas");
            ModelState.Remove("ProgresoUsuarios");
            if (ModelState.IsValid)
            {
                await this.repo.UpdateLeccionAsync(leccion);

                TempData["Titulo"] = "¡Lección actualizada!";
                TempData["Mensaje"] = "Los cambios se han guardado correctamente.";
                TempData["Icono"] = "success";

                return RedirectToAction("Details", "Cursos", new { id = leccion.CursoID });
            }
            return View(leccion);
        }
    }

}
