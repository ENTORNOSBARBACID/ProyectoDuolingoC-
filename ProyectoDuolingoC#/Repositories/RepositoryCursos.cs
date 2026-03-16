using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Protocol.Plugins;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;
using System.Numerics;

namespace ProyectoDuolingoC_.Repositories
{
    #region procedures
        //    CREATE PROCEDURE SP_EliminarCursoEnCascada
        //    @CursoID INT
        //AS
        //BEGIN
        //    SET NOCOUNT ON;

        //    BEGIN TRY
        //        --Iniciamos la transacción: Todo lo que pase a partir de aquí es "temporal" hasta el COMMIT
        //        BEGIN TRANSACTION;

        //        -- 1. Borramos el progreso de los usuarios(depende de las lecciones del curso)
        //        DELETE FROM dbo.ProgresoUsuario
        //        WHERE LeccionID IN(SELECT LeccionID FROM dbo.Lecciones WHERE CursoID = @CursoID);

        //        -- 2. Borramos las opciones de respuesta(dependen de las preguntas, que dependen de las lecciones)
        //        DELETE FROM dbo.OpcionesRespuesta
        //        WHERE PreguntaID IN(
        //            SELECT PreguntaID FROM dbo.Preguntas
        //            WHERE LeccionID IN (SELECT LeccionID FROM dbo.Lecciones WHERE CursoID = @CursoID)
        //        );

        //        -- 3. Borramos las preguntas(dependen de las lecciones)
        //        DELETE FROM dbo.Preguntas
        //        WHERE LeccionID IN(SELECT LeccionID FROM dbo.Lecciones WHERE CursoID = @CursoID);

        //        -- 4. Borramos las lecciones en sí
        //        DELETE FROM dbo.Lecciones
        //        WHERE CursoID = @CursoID;

        //        -- 5. Borramos a los usuarios que estaban apuntados al curso
        //        DELETE FROM dbo.CursosUsuario
        //        WHERE CursoID = @CursoID;

        //        -- 6. Finalmente, como ya no tiene "hijos" que lo aten, borramos el curso
        //        DELETE FROM dbo.Cursos
        //        WHERE CursoID = @CursoID;

        //        -- Si el código sobrevive hasta esta línea sin explotar, consolidamos los cambios
        //        COMMIT TRANSACTION;
        //    END TRY
        //    BEGIN CATCH
        //        -- ¡ALERTA! Algo falló(ej: se cayó el servidor a mitad o hubo un error de FK)
        
        //        -- Si hay una transacción a medias, la DESHACEMOS por completo
        //        IF @@TRANCOUNT > 0
        //        BEGIN
        //            ROLLBACK TRANSACTION;
        //        END

        //        -- Le lanzamos el error de vuelta a C# para que sepa exactamente qué pasó
        //        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        //    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        //    DECLARE @ErrorState INT = ERROR_STATE();

        //    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        //    END CATCH
        //END
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
