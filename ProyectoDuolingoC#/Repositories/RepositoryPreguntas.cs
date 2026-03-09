using Microsoft.EntityFrameworkCore;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;

namespace ProyectoDuolingoC_.Repositories
{
    public class RepositoryPreguntas
    {
        private ProyectoContext context;
        public RepositoryPreguntas(ProyectoContext context)
        {
            this.context = context;
        }

        public async Task<List<Pregunta>> VerPregunta(int idLeccion)
        {
            var pregunta = await context.Pregunta
                .Where(l => l.LeccionID == idLeccion)
                .ToListAsync();
            return pregunta;
        }
        public async Task<List<OpcionRespuesta>> VerOpciones(int idPregunta)
        {
            var opciones = await context.OpcionRespuesta
                .Where(o => o.PreguntaID == idPregunta)
                .ToListAsync();
            return opciones;
        }
        public async Task<Pregunta> VerPreguntaPorId(int idPregunta)
        {
            var pregunta = await context.Pregunta
                .Where(p => p.PreguntaID == idPregunta)
                .FirstOrDefaultAsync();
            return pregunta;
        }

        public async Task<List<Pregunta>> VerPreguntasPorLeccion(int leccionId)
        {
            var preguntas = await context.Pregunta
                .Where(p => p.LeccionID == leccionId)
                .ToListAsync();
            return preguntas;
        }
    }
}
