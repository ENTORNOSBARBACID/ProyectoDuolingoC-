using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuggetLanguoABF.Models;
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
            return View(new Usuario());
        }
        [HttpPost]
        public async Task<IActionResult> Register(Usuario user, string pass, IFormFile archivoImagen)
        {
            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await archivoImagen.CopyToAsync(memoryStream);

                    user.Imagen = memoryStream.ToArray();
                }
            }
            await this.repo.RegisterUsuario(user.NombreUsuario, user.CorreoElectronico, user.Imagen, user.Rol, pass);
            ViewData["MENSAJE"] = "Usuario en el sistema";
            return RedirectToAction("LogIn", new { email = user.CorreoElectronico, pass= pass});
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetFotoPerfil()
        {
            // 1. Recuperamos el string Base64 de la Session (no de User)
            string fotoBase64 = HttpContext.Session.GetString("FOTO");

            if (string.IsNullOrEmpty(fotoBase64))
            {
                // Si no hay foto, devolvemos un 404
                return NotFound();
            }

            // 2. Convertimos el string Base64 de nuevo a un array de bytes
            // Esto es necesario porque el método File() necesita bytes para "dibujar" la imagen
            byte[] imagenBytes = Convert.FromBase64String(fotoBase64);

            // 3. Devolvemos el archivo indicando el tipo de contenido
            return File(imagenBytes, "image/jpeg");
        }
        public async Task<IActionResult> LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string pass)
        {
            LoginResponseDTO response = await this.repo.LogInUserAsync(email, pass);

            if (response == null)
            {
                ViewData["MENSAJE"] = "Credenciales no válidas";
                return View();
            }
            else
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(response.Token);

                ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role);
                Usuario Usuario = await this.repo.GetPerfilAsync(response.Token);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Usuario.UsuarioID.ToString()));
                identity.AddClaim(new Claim("JWT", response.Token));
                identity.AddClaim(new Claim(ClaimTypes.Role, Usuario.Rol.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, Usuario.NombreUsuario));

                if (Usuario.Imagen != null)
                {
                    string fotoBase64 = Convert.ToBase64String(Usuario.Imagen);
                    HttpContext.Session.SetString("FOTO", fotoBase64);
                }

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString("TOKEN", response.Token);

                if (response.ImagenPerfil != null)
                {
                    string base64Image = Convert.ToBase64String(response.ImagenPerfil);
                    HttpContext.Session.SetString("FOTO", base64Image);
                }

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
            Usuario usu = await this.repo.FindUsuarioByIDAsync(int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            return View(usu);
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

        [Authorize] // Imprescindible para que solo entren usuarios logueados
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            // 1. Sacamos el ID del usuario directamente de su "carnet de identidad" (Claims)
            string claimId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(claimId))
            {
                return RedirectToAction("LogIn", "Autenticacion");
            }

            // 2. Buscamos sus datos y se los mandamos a tu vista bonita
            int idUsu = int.Parse(claimId);
            Usuario user = await this.repo.FindUsuarioByIDAsync(idUsu);

            return View(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(Usuario usuarioModificado, IFormFile imagenArchivo)
        {
            byte[] imagenBytes = null;
            if (imagenArchivo != null && imagenArchivo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await imagenArchivo.CopyToAsync(ms);
                    imagenBytes = ms.ToArray();
                }
            }
            int idLogueado = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (usuarioModificado.UsuarioID != idLogueado)
            {
                return RedirectToAction("LogIn", "Autenticacion");
            }

            await this.repo.UpdatePerfilAsync(usuarioModificado.UsuarioID, usuarioModificado.NombreUsuario, imagenBytes);
            TempData["MENSAJE"] = "¡Perfil actualizado con éxito!";
            TempData["TIPO_MENSAJE"] = "success";

            return RedirectToAction("VerPerfil");
        }

        public async Task<IActionResult> Ranking()
        {
            List<Usuario> usu = await this.repo.ObtenerRankingGlobalAsync();
            return View(usu);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
