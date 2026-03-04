using Microsoft.EntityFrameworkCore;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;

namespace ProyectoDuolingoC_.Repositories
{
    public class RepositoryLecciones
    {
        private ProyectoContext context;
        public RepositoryLecciones(ProyectoContext context)
        {
            this.context = context;
        }
        public async Task<List<Leccion>> LoadLecciones(int id)
        {
            var consulta = from datos in this.context.Leccion
                           where datos.CursoID == id
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<ProgresoUsuario> VerProgresoUsuario(int idUsu, int idLec)
        {
            var consulta = from datos in this.context.ProgresoUsuario
                           where datos.UsuarioID == idUsu && datos.LeccionID == idLec
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        public async Task<string> VerContenido(int idLeccion)
        {
            var leccion = await context.Leccion
                .Where(l => l.LeccionID == idLeccion)
                .Select(l => l.ContenidoTeorico)
                .FirstOrDefaultAsync();

            // Si no existe la lección o el contenido es nulo, devolvemos un mensaje amigable
            return leccion ?? "No hay contenido disponible para esta lección todavía.";
        }
    }
}
