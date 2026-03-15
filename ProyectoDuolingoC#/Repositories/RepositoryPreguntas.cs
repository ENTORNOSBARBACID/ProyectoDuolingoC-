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
        public async Task ActualizarPregunta(Pregunta pregunta)
        {
            this.context.Pregunta.Update(pregunta);
            await this.context.SaveChangesAsync();
        }
        public async Task CrearPregunta(Pregunta pregunta)
        {
            await this.context.Pregunta.AddAsync(pregunta);
            await this.context.SaveChangesAsync();
        }
        public async Task InsertarOpcion(int id, string texto)
        {
            OpcionRespuesta nuevaOpcion = new OpcionRespuesta
            {
                PreguntaID = id,
                TextoOpcion = texto
            };
            await this.context.OpcionRespuesta.AddAsync(nuevaOpcion);
            await this.context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            Pregunta p = await VerPreguntaPorId(id);
            if (p != null)
            {
                // 3. La marcamos para borrar (¡Sin el await!)
                this.context.Pregunta.Remove(p);

                // 4. Guardamos los cambios de forma asíncrona
                await this.context.SaveChangesAsync();
            }
        }
        public async Task EliminarOpcion(int id)
        {
            OpcionRespuesta o = await context.OpcionRespuesta
                .Where(op => op.OpcionID == id)
                .FirstOrDefaultAsync();
            if (o != null)
            {
                this.context.OpcionRespuesta.Remove(o);
                await this.context.SaveChangesAsync();
            }
        }
    }
}
