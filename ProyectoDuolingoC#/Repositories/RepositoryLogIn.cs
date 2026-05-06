using NuggetLanguoABF.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http.Headers; 
namespace ProyectoDuolingoC_.Repositories
{
    public class RepositoryLogIn
    {
        private HttpClient client;
        private string urlApi;
        private IHttpContextAccessor httpContextAccessor;

        public RepositoryLogIn(HttpClient client, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            this.urlApi = configuration.GetValue<string>("ApisUrl:ApiProyecto");

            this.client.BaseAddress = new Uri(this.urlApi);
            this.client.DefaultRequestHeaders.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task RegisterUsuario(string nombre, string email, byte[] imagen, int rol, string password)
        {
            string request = "api/LogIn/RegisterUsuario";

            // Empaquetamos los datos en el DTO
            RegistroUsuarioDTO model = new RegistroUsuarioDTO
            {
                Nombre = nombre,
                Email = email,
                Imagen = imagen,
                Rol = rol,
                Password = password
            };

            await this.client.PostAsJsonAsync(request, model);
        }

        public async Task<LoginResponseDTO> LogInUserAsync(string email, string password)
        {
            string request = "api/LogIn/LogInUser";

            LoginUsuarioDTO model = new LoginUsuarioDTO
            {
                Email = email,
                Password = password
            };

            var response = await this.client.PostAsJsonAsync(request, model);

            if (response.IsSuccessStatusCode)
            {
                // Atrapamos el Token y la Imagen al mismo tiempo
                return await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
            }

            return null;
        }


    public async Task<Usuario> GetPerfilAsync(string token)
        {
            string request = "api/LogIn/Perfil";

            this.client.DefaultRequestHeaders.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            this.client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await this.client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Usuario user = await response.Content.ReadFromJsonAsync<Usuario>();
                return user;
            }
            else
            {
                return null;
            }
        }

    public async Task<Usuario> FindUsuarioByIDAsync(int id)
        {
            string request = $"api/LogIn/FindUsuarioByID/{id}";
            var response = await this.client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Usuario>();
            }

            return null;
        }

        public async Task UpdatePerfilAsync(int idUsuario, string nuevoNombre, byte[] nuevaImagen)
        {
            string request = "api/LogIn/UpdatePerfil";

            UpdatePerfilDTO model = new UpdatePerfilDTO
            {
                IdUsuario = idUsuario,
                NuevoNombre = nuevoNombre,
                NuevaImagen = nuevaImagen
            };

            await this.client.PutAsJsonAsync(request, model);
        }

        public async Task<List<Usuario>> ObtenerRankingGlobalAsync()
        {
            string request = "api/LogIn/ObtenerRankingGlobal";
            var response = await this.client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Usuario>>();
            }

            return new List<Usuario>();
        }
    }

    // --- Clases DTO para enviar la información a la API ---
    // Deben coincidir exactamente con las que creamos en el LogInController de la API

    public class RegistroUsuarioDTO
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public byte[] Imagen { get; set; }
        public int Rol { get; set; }
        public string Password { get; set; }
    }

    public class LoginUsuarioDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdatePerfilDTO
    {
        public int IdUsuario { get; set; }
        public string NuevoNombre { get; set; }
        public byte[] NuevaImagen { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public byte[] ImagenPerfil { get; set; }
    }
}