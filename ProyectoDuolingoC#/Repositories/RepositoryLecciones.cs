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
        public async Task<ProgresoUsuario> VerProgresoUsuarioAsync(int idUsu, int idLec)
        {
            var consulta = from datos in this.context.ProgresoUsuario
                           where datos.UsuarioID == idUsu && datos.LeccionID == idLec
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        public async Task<List<ProgresoUsuario>> VerProgresoUsuarioListAsync(int idUsu, int idCur)
        {
            var consulta = from datos in this.context.ProgresoUsuario
                           where datos.UsuarioID == idUsu && datos.CursoID == idCur
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<Leccion> VerContenido(int idLeccion)
        {
            var leccion = await context.Leccion
                .Where(l => l.LeccionID == idLeccion)
                .FirstOrDefaultAsync();

            return leccion;
        }
        public async Task<int> GetOrderAsync(int cursoId)
        {
            var consulta = await context.Leccion
                .Where(l => l.CursoID == cursoId)
                .OrderByDescending(l => l.Orden)
                .FirstOrDefaultAsync();
            if (consulta == null)
            {
                return 1;
            }
            return consulta.Orden + 1;
        }
        public async Task CreateLeccionAsync(Leccion lec)
        {
            lec.Orden = await GetOrderAsync(lec.CursoID);
            await this.context.Leccion.AddAsync(lec);

            await this.context.SaveChangesAsync();
        }
        public async Task ImplementUsuarioProgreso(int idUsu, int idLec, int idCur)
        {
            if(await VerProgresoUsuarioAsync(idUsu, idLec) == null)
            {
                ProgresoUsuario usuP = new ProgresoUsuario
                {
                    UsuarioID = idUsu,
                    LeccionID = idLec,
                    CursoID = idCur,
                    FechaCompletado = DateTime.Now
                };
                await this.context.ProgresoUsuario.AddAsync(usuP);
                await this.context.SaveChangesAsync();
            }
        }
    }
}
