using NuggetLanguoABF.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProyectoDuolingoC_.Repositories
{
    public class RepositoryLecciones
    {
        private HttpClient client;
        private string urlApi;
        private IHttpContextAccessor httpContextAccessor;

        public RepositoryLecciones(HttpClient client, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            // Leemos la URL base desde tu appsettings.json
            this.urlApi = configuration.GetValue<string>("ApisUrl:ApiProyecto");
            this.httpContextAccessor = httpContextAccessor;
            this.client.BaseAddress = new Uri(this.urlApi);
            this.client.DefaultRequestHeaders.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void AddTokenToHeader()
        {
            string token = this.httpContextAccessor.HttpContext.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                this.client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<Leccion>> LoadLecciones(int id)
        {
            string request = $"api/Lecciones/LoadLecciones/{id}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Leccion>>();
            }
            return new List<Leccion>();
        }

        public async Task<ProgresoUsuario> VerProgresoUsuarioAsync(int idUsu, int idLec)
        {
            AddTokenToHeader();
            string request = $"api/Lecciones/VerProgresoUsuario/{idUsu}/{idLec}";
            var response = await this.client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProgresoUsuario>();
            }
            return null;
        }

        public async Task<List<ProgresoUsuario>> VerProgresoUsuarioListAsync(int idUsu, int idCur)
        {
            AddTokenToHeader();
            string request = $"api/Lecciones/VerProgresoUsuarioList/{idUsu}/{idCur}";
            var response = await this.client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProgresoUsuario>>();
            }
            return new List<ProgresoUsuario>();
        }

        public async Task<ProgresoUsuario> VerProgresoUsuarioLastAsync(int idUsu, int idCur)
        {
            AddTokenToHeader();
            string request = $"api/Lecciones/VerProgresoUsuarioLast/{idUsu}/{idCur}";
            var response = await this.client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProgresoUsuario>();
            }
            return null;
        }

        public async Task<Leccion> VerContenido(int idLeccion)
        {
            string request = $"api/Lecciones/VerContenido/{idLeccion}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Leccion>();
            }
            return null;
        }

        public async Task<int> GetOrderAsync(int cursoId)
        {
            string request = $"api/Lecciones/GetOrder/{cursoId}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            return 1; // Retorna 1 por defecto si falla, igual que en tu lógica original
        }

        public async Task CreateLeccionAsync(Leccion lec)
        {
            // Añade el token aquí si tienes la API protegida con [Authorize]
            string token = this.httpContextAccessor.HttpContext.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                this.client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            string request = "api/Lecciones/CreateLeccion";

            var response = await this.client.PostAsJsonAsync(request, lec);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"La API ha fallado al crear la lección. Código: {response.StatusCode}. Detalle: {error}");
            }
        }

        public async Task ImplementUsuarioProgreso(int idUsu, int idLec, int idCur)
        {
            string request = $"api/Lecciones/ImplementUsuarioProgreso/{idUsu}/{idLec}/{idCur}";
            // Se envía un POST vacío porque los IDs ya viajan en la propia URL
            await this.client.PostAsync(request, null);
        }

        public async Task EliminarLeccionEnCascada(int idLeccion)
        {
            string request = $"api/Lecciones/EliminarLeccionEnCascada/{idLeccion}";
            await this.client.DeleteAsync(request);
        }

        public async Task UpdateLeccionAsync(Leccion leccionModificada)
        {
            string request = "api/Lecciones/UpdateLeccion";
            await this.client.PutAsJsonAsync(request, leccionModificada);
        }
    }
}