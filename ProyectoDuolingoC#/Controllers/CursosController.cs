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

            int idUsu = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (idUsu != null)
            {
                CursosUsuario cursosUsuario = await this.repo.VerCursousuarioAsync(id, idUsu);
                List<ProgresoUsuario> progreso = await this.repoLec.VerProgresoUsuarioListAsync(idUsu, id);
                if(progreso != null && progreso.Any())
                {
                    ViewData["LECCION"] = progreso.Count() + 1;
                }
                else if (HttpContext.User.FindFirst(ClaimTypes.Role).Value == "2")
                {
                    ViewData["LECCION"] = lec.Count() + 1;
                }
                else
                {
                    ViewData["LECCION"] = 1;
                }
                if(HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
                {
                    ViewData["CURSO"] = cursosUsuario == null;
                }
                else
                {
                    ViewData["CURSO"] = false;
                }
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
            int idUsu = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (idUsu == null)
            {
                TempData["Titulo"] = "¡Ups!";
                TempData["Mensaje"] = "Debes iniciar sesión para poder inscribirte en este curso.";
                TempData["Icono"] = "warning";
                return RedirectToAction("Login", "Home");
            }

            
            await this.repo.Inscribirse(id, idUsu);
            
            TempData["Titulo"] = "¡Genial!";
            TempData["Mensaje"] = "Te has inscrito al curso correctamente.";
            TempData["Icono"] = "success"; 


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
        public async Task<IActionResult> Update(int id)
        {
            Curso curso = await this.repo.FindCurso(id);
            return View(curso);
        }
        [HttpPost]
        [Authorize(policy: ("SOLOADMIN"))]
        public async Task<IActionResult> Update(Curso curso, IFormFile archivoImagen)
        {
            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await archivoImagen.CopyToAsync(memoryStream);

                    curso.Imagen = memoryStream.ToArray();
                }
            }
            await this.repo.UpdateCursoAsync(curso);
            TempData["MENSAJE"] = "¡Curso actualizado con éxito!";
            TempData["TIPO_MENSAJE"] = "success";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(policy: "SOLOESTUDIANTES")]
        [HttpGet]
        public async Task<IActionResult> MisCursos()
        {
            int id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<CursoProgresoVM> curso = await this.repo.GetMisCursosConProgreso(id);
            return View(curso);
        }

            [Authorize(Policy = "SOLOESTUDIANTES")]
            public async Task<IActionResult> ContinuarAvanzando(int idCurso)
            {
                int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                ProgresoUsuario ultimoProgreso = await this.repoLec.VerProgresoUsuarioLastAsync(idUsuario, idCurso);
                List<Leccion> lec = await this.repoLec.LoadLecciones(idCurso);

                int idLeccionDestino = 0;
                if (lec != null && lec.Any())
                {

                    if (ultimoProgreso == null)
                    {

                        idLeccionDestino = await this.repo.GetPrimeraLeccionCursoAsync(idCurso);
                    }
                    else
                    {
                        idLeccionDestino = await this.repo.GetSiguienteLeccionAsync(idCurso, ultimoProgreso.LeccionID);

                        if (idLeccionDestino == 0)
                        {
                            TempData["Titulo"] = "¡Curso Completado!";
                            TempData["Mensaje"] = "¡Enhorabuena! Apúntate a otros cursos para seguir aprendiendo.";
                            TempData["Icono"] = "success"; 
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    return RedirectToAction("Index", "Lecciones", new { id = idLeccionDestino });
                }
                else
                {
                    TempData["Titulo"] = "No hay cursos disponibles todavia...";
                    TempData["Mensaje"] = "¡Espera paciente! Pronto publicaremos las lecciones de este curso.";
                    TempData["Icono"] = "error";
                    return RedirectToAction("MisCursos", "Cursos");
            }
            }
        public async Task<IActionResult> VerEstudiantes()
        {
            List<EstudianteAdminVM> listaEstudiantes = await this.repo.VerEstudiantes();
            return View(listaEstudiantes);
        }

        public async Task<IActionResult> ExpulsarEstudiante(int idUsuario, int idCurso)
        {
           return RedirectToAction("VerEstudiantes");
        }

    }
}
