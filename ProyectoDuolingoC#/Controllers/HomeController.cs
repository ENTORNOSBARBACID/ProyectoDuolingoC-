using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProyectoDuolingoC_.Models;
using ProyectoDuolingoC_.Repositories;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

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
            return RedirectToAction("LogIn", new { email = user.CorreoElectronico, pass= pass});
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
                ViewData["MENSAJE"] = "Credenciales no válidas";
                return View();
            }
            else
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role);

                Claim claimId = new Claim(ClaimTypes.NameIdentifier, user.UsuarioID.ToString());
                Claim claimName = new Claim(ClaimTypes.Name, user.NombreUsuario);

                Claim claimRole = new Claim(ClaimTypes.Role, user.Rol.ToString());

                identity.AddClaim(claimId);
                identity.AddClaim(claimName);
                identity.AddClaim(claimRole);

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetInt32("ID", user.UsuarioID);

                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("ID");
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> VerPerfil()
        {
            Usuario usu = await this.repo.FindUsuarioByIDAsync(HttpContext.Session.GetInt32("ID").Value);
            return View(usu);
        }
        public async Task<IActionResult> MisCursos()
        {
            List<CursoProgresoVM> curso = await this.repoCursos.GetMisCursosConProgreso(HttpContext.Session.GetInt32("ID").Value);
            return View(curso);
        }

        [HttpGet]
        public IActionResult ErrorAcceso()
        {
            TempData["AccesoDenegado"] = "No tienes los permisos necesarios para acceder a esta seccion.";

            string urlAnterior = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(urlAnterior))
            {
                return Redirect(urlAnterior);
            }

            return RedirectToAction("Index", "Home");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
