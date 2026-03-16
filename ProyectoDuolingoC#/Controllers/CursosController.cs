using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoDuolingoC_.Models;
using ProyectoDuolingoC_.Repositories;
using System.Security.Claims;
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

            int? idUsu = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (idUsu != null)
            {
                CursosUsuario cursosUsuario = await this.repo.VerCursousuarioAsync(id, idUsu.Value);
                List<ProgresoUsuario> progreso = await this.repoLec.VerProgresoUsuarioListAsync(idUsu.Value);
                if(progreso != null && progreso.Any())
                {
                    ViewData["LECCION"] = progreso.Last().LeccionID ;
                }
                else
                {
                    ViewData["LECCION"] = 3;
                }
                ViewData["CURSO"] = cursosUsuario == null;
            }
            else
            {
                ViewData["CURSO"] = true;
            }

            return View(curso);
        }
        [Authorize(policy:"SOLOESTUDIANTES")]
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
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "SOLOADMIN")] // Mantenemos tu seguridad a tope
        public async Task<IActionResult> Create(Curso curso, IFormFile archivoImagen)
        {
            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await archivoImagen.CopyToAsync(memoryStream);

                    curso.Imagen = memoryStream.ToArray();
                }
            }

            await this.repo.CreateCursoAsync(curso);

            return RedirectToAction("Index", "Home");
        }
        [Authorize(policy: ("SOLOADMIN"))]
        public async Task<IActionResult> Delete(int id)
        {
            await this.repo.Delete(id);
            return RedirectToAction("Index", "Home");
        }

    }
}
