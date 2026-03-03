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
    }
}
