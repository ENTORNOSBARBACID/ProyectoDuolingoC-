using NuggetLanguoABF.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProyectoDuolingoC_.Repositories
{
    public class RepositoryCursos
    {
        private HttpClient client;
        private string urlApi;

        public RepositoryCursos(HttpClient client, IConfiguration configuration)
        {
            this.client = client;
            // Leemos la URL base desde tu appsettings.json
            this.urlApi = configuration.GetValue<string>("ApisUrl:ApiProyecto");

            this.client.BaseAddress = new Uri(this.urlApi);
            this.client.DefaultRequestHeaders.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Curso>> LoadCursos()
        {
            string request = "api/Cursos/LoadCursos";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Curso>>();
            }
            return new List<Curso>();
        }

        public async Task<Curso> FindCurso(int idCurso)
        {
            string request = $"api/Cursos/FindCurso/{idCurso}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Curso>();
            }
            return null;
        }

        public async Task Inscribirse(int idCurso, int idUsuario)
        {
            string request = $"api/Cursos/Inscribirse/{idCurso}/{idUsuario}";
            await this.client.PostAsync(request, null);
        }

        public async Task<CursosUsuario> VerCursousuarioAsync(int idCurso, int idUsuario)
        {
            string request = $"api/Cursos/VerCursousuario/{idCurso}/{idUsuario}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return null;
                }

                string jsonContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    return null;
                }
                return await response.Content.ReadFromJsonAsync<CursosUsuario>();
            }
            return null;
        }

        public async Task CreateCursoAsync(Curso c)
        {
            string request = "api/Cursos/CreateCurso";
            await this.client.PostAsJsonAsync(request, c);
        }

        public async Task CreateCursos(Curso c)
        {
            await CreateCursoAsync(c);
        }

        public async Task UpdateCursoAsync(Curso cursoModificado)
        {
            string request = "api/Cursos/UpdateCurso";
            await this.client.PutAsJsonAsync(request, cursoModificado);
        }

        public async Task Delete(int id)
        {
            string request = $"api/Cursos/DeleteCurso/{id}";
            var response = await this.client.GetAsync(request);

            
        }

        public async Task<List<CursoProgresoVM>> GetMisCursosConProgreso(int usuarioId)
        {
            string request = $"api/Cursos/GetMisCursosConProgreso/{usuarioId}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CursoProgresoVM>>();
            }
            return new List<CursoProgresoVM>();
        }

        public async Task<int> GetPrimeraLeccionCursoAsync(int idCurso)
        {
            string request = $"api/Cursos/GetPrimeraLeccionCurso/{idCurso}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            return 0;
        }

        public async Task<int> GetSiguienteLeccionAsync(int idCurso, int idLeccionActual)
        {
            string request = $"api/Cursos/SiguienteLeccion/{idCurso}/{idLeccionActual}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            return 0;
        }

        public async Task<List<EstudianteAdminVM>> VerEstudiantes()
        {
            string request = "api/Cursos/VerEstudiantes";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EstudianteAdminVM>>();
            }
            return new List<EstudianteAdminVM>();
        }

        public async Task ExpulsarEstudianteCurso(int idUsuario, int idCurso)
        {
            string request = $"api/Cursos/ExpulsarEstudiante/{idUsuario}/{idCurso}";
            await this.client.DeleteAsync(request);
        }
    }
}