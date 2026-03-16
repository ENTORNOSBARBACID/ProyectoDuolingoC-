using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;

namespace ProyectoDuolingoC_.Repositories
{
    public class RepositoryCursos
    {
        private ProyectoContext context;
        public RepositoryCursos(ProyectoContext context)
        {
            this.context = context;
        }

        public async Task<List<Curso>> LoadCursos()
        {
            var consulta = from datos in this.context.Curso
                           select datos;

            return await consulta.ToListAsync();
        }
        public async Task CreateCursos(Curso c)
        {

        }
        public async Task<Curso> FindCurso(int CursoID)
        {
            var consulta = from datos in this.context.Curso
                           where datos.CursoID == CursoID
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        public async Task Inscribirse(int idCurso, int idUsuario)
        {
            bool yaInscrito = await this.context.CursosUsuarios
                                              .AnyAsync(cu => cu.CursoID == idCurso && cu.UsuarioID == idUsuario);

            if (!yaInscrito)
            {
                CursosUsuario curUsu = new CursosUsuario()
                {
                    UsuarioID = idUsuario,
                    CursoID = idCurso
                };

                await this.context.CursosUsuarios.AddAsync(curUsu);
                await this.context.SaveChangesAsync();
            }
        }
        public async Task<CursosUsuario> VerCursousuarioAsync(int idCurso, int idUsuario)
        {
            var consulta = from datos in this.context.CursosUsuarios
                           where datos.CursoID.Equals(idCurso) && datos.UsuarioID.Equals(idUsuario)
                           select datos;

            return await consulta.FirstOrDefaultAsync();
        }
        public async Task CreateCursoAsync(Curso c)
        {
            await this.context.Curso.AddAsync(c);
            await this.context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {

            string sql = "SP_EliminarCursoEnCascada @CursoID";
            SqlParameter pamId = new SqlParameter("CursoID", id);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamId);
            await this.context.SaveChangesAsync();
        }

        
    }
}
