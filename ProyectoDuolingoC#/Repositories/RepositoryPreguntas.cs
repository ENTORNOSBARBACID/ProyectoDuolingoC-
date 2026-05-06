using Azure;
using NuggetLanguoABF.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProyectoDuolingoC_.Repositories
{
    public class RepositoryPreguntas
    {
        private HttpClient client;
        private string urlApi;

        public RepositoryPreguntas(HttpClient client, IConfiguration configuration)
        {
            this.client = client;
            // Leemos la URL base desde tu appsettings.json
            this.urlApi = configuration.GetValue<string>("ApisUrl:ApiProyecto");

            this.client.BaseAddress = new Uri(this.urlApi);
            this.client.DefaultRequestHeaders.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Pregunta>> VerPregunta(int idLeccion)
        {
            string request = $"api/Preguntas/VerPregunta/{idLeccion}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Pregunta>>();
            }
            return new List<Pregunta>();
        }

        public async Task<List<OpcionRespuesta>> VerOpciones(int idPregunta)
        {
            string request = $"api/Preguntas/VerOpciones/{idPregunta}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<OpcionRespuesta>>();
            }
            return new List<OpcionRespuesta>();
        }

        public async Task<Pregunta> VerPreguntaPorId(int idPregunta)
        {
            string request = $"api/Preguntas/VerPreguntaPorId/{idPregunta}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Pregunta>();
            }
            return null;
        }

        public async Task<List<Pregunta>> VerPreguntasPorLeccion(int leccionId)
        {
            string request = $"api/Preguntas/VerPreguntasPorLeccion/{leccionId}";
            var response = await this.client.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Pregunta>>();
            }
            return new List<Pregunta>();
        }

        public async Task ActualizarPregunta(Pregunta pregunta)
        {
            string request = "api/Preguntas/ActualizarPregunta";
            await this.client.PutAsJsonAsync(request, pregunta);
        }

        public async Task<Pregunta> CrearPregunta(Pregunta pregunta)
        {
            string request = "api/Preguntas/CreatePregunta";
            var response = await this.client.PostAsJsonAsync(request, pregunta);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Pregunta>();
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error en la API al crear pregunta: {response.StatusCode}. Detalle: {error}");
            }
        }

        public async Task InsertarOpcion(int id, string texto)
        {
            string request = "api/Preguntas/InsertarOpcion";

            // Empaquetamos los datos para enviarlos de forma segura en el body
            InsertarOpcionDTO model = new InsertarOpcionDTO
            {
                IdPregunta = id,
                TextoOpcion = texto
            };

            await this.client.PostAsJsonAsync(request, model);
        }

        public async Task EliminarOpcion(int id)
        {
            string request = $"api/Preguntas/EliminarOpcion/{id}";
            await this.client.DeleteAsync(request);
        }

        public async Task EliminarPregunta(int id)
        {
            string request = $"api/Preguntas/EliminarPregunta/{id}";
            await this.client.DeleteAsync(request);
        }

        public async Task SumarPuntos(int puntos, int idUsuario)
        {
            // Ojo al orden de los parámetros en la ruta para que coincida con tu controlador: {idUsuario}/{puntos}
            string request = $"api/Preguntas/SumarPuntos/{idUsuario}/{puntos}";
            // Enviamos un PUT vacío porque la información ya va en la URL
            await this.client.PutAsync(request, null);
        }
    }

    // --- DTO para el envío de opciones ---
    public class InsertarOpcionDTO
    {
        public int IdPregunta { get; set; }
        public string TextoOpcion { get; set; }
    }
}