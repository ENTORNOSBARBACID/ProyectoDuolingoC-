using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace ProyectoDuolingoC_.Repositories
{
    #region procedures
//    CREATE PROCEDURE SP_ObtenerProgresoMisCursos
//    @UsuarioID INT
//AS
//BEGIN
//    SET NOCOUNT ON;

//    SELECT
//        cu.CursoID,
//        -- Traemos también la info del curso por si quieres rellenar la tarjeta directamente con este Procedure
//        c.Titulo,
//        c.Descripcion,
//        c.EtiquetaLenguaje,
        
//        -- Cálculo del porcentaje
//        CASE 
//            -- Si el curso no tiene lecciones creadas todavía, el progreso es 0%
//            WHEN ISNULL(TotalLec.Total, 0) = 0 THEN 0
            
//            -- Calculamos: (Completadas / Totales) * 100
//            ELSE CAST(ROUND((CAST(ISNULL(LecCompletadas.Cantidad, 0) AS FLOAT) / TotalLec.Total) * 100, 0) AS INT)
//        END AS PorcentajeProgreso

//    FROM dbo.CursosUsuario cu
    
//    -- Unimos con la tabla de Cursos para sacar la información básica
//    INNER JOIN dbo.Cursos c ON cu.CursoID = c.CursoID

//    -- Subconsulta 1: Sacamos el total de lecciones que tiene cada curso
//    LEFT JOIN (
//        SELECT CursoID, COUNT(LeccionID) AS Total
//        FROM dbo.Lecciones
//        GROUP BY CursoID
//    ) AS TotalLec ON cu.CursoID = TotalLec.CursoID
    
//    -- Subconsulta 2: Sacamos cuántas lecciones ha completado ESTE usuario en cada curso
//    LEFT JOIN (
//        SELECT l.CursoID, COUNT(pu.LeccionID) AS Cantidad
//        FROM dbo.ProgresoUsuario pu
//        INNER JOIN dbo.Lecciones l ON pu.LeccionID = l.LeccionID
//        WHERE pu.UsuarioID = @UsuarioID
//        GROUP BY l.CursoID
//    ) AS LecCompletadas ON cu.CursoID = LecCompletadas.CursoID
    
//    -- Filtramos para que solo salgan los cursos a los que está apuntado el usuario
//    WHERE cu.UsuarioID = @UsuarioID;
//    END

    #endregion

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
        public async Task<List<Curso>> LoadCursosUsuario(int idUsuario)
        {
            var consulta = from datos in this.context.CursosUsuarios
                           where datos.UsuarioID == idUsuario
                           select datos.CursoID;
            var consultaCursos = from datos in this.context.Curso
                           where consulta.Contains(datos.CursoID)
                           select datos;
            return await consultaCursos.ToListAsync();
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

        public async Task<List<CursoProgresoVM>> GetMisCursosConProgreso(int usuarioId)
        {
            string sql = "EXEC SP_ObtenerProgresoMisCursos @UsuarioID";
            SqlParameter pamusu = new SqlParameter("@UsuarioID", usuarioId);

            // Ahora usamos el nuevo DbSet que creamos
            var misCursos = await this.context.CursosConProgreso
                .FromSqlRaw(sql, pamusu)
                .ToListAsync();

            return misCursos;
        }
    }
}
