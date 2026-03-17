using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Protocol.Plugins;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProyectoDuolingoC_.Repositories
{
    #region procedures
    //    CREATE PROCEDURE SP_EliminarLeccionEnCascada
    //    @LeccionID INT
    //AS
    //BEGIN
    //    SET NOCOUNT ON;

    //    -- 1. Capturamos a qué curso pertenece esta lección ANTES de borrarla
    //    DECLARE @CursoID INT;
    //    SELECT @CursoID = CursoID FROM dbo.Lecciones WHERE LeccionID = @LeccionID;

    //    BEGIN TRY
    //        BEGIN TRAN;

    //        -- 2. Borramos Opciones(Nietos)
    //        DELETE FROM dbo.OpcionesRespuesta
    //        WHERE PreguntaID IN(
    //            SELECT PreguntaID FROM dbo.Preguntas WHERE LeccionID = @LeccionID
    //        );

    //        -- 3. Borramos Preguntas(Hijos)
    //        DELETE FROM dbo.Preguntas
    //        WHERE LeccionID = @LeccionID;

    //        -- 4. Borramos Progreso(Hijos)
    //        DELETE FROM dbo.ProgresoUsuario
    //        WHERE LeccionID = @LeccionID;

    //        -- 5. Borramos la Lección(Padre)
    //        DELETE FROM dbo.Lecciones
    //        WHERE LeccionID = @LeccionID;

    //        -- 6. LA MAGIA DEL REORDENAMIENTO
    //        --Solo lo hacemos si encontramos el curso válido
    //        IF @CursoID IS NOT NULL
    //        BEGIN
    //            -- Usamos una CTE(Tabla Temporal en Memoria) para enumerar las lecciones que quedan 1, 2, 3...
    //            WITH LeccionesReordenadas AS(
    //                SELECT
    //                    LeccionID,
    //                    Orden,
    //                    -- ROW_NUMBER genera una secuencia perfecta (1,2,3...) basándose en el orden que ya tenían
    //                    ROW_NUMBER() OVER(ORDER BY Orden ASC, LeccionID ASC) AS NuevoOrden
    //                FROM dbo.Lecciones
    //                WHERE CursoID = @CursoID
    //            )
    //            -- Actualizamos la tabla real pisando el Orden viejo con el NuevoOrden perfecto
    //            UPDATE LeccionesReordenadas
    //            SET Orden = NuevoOrden
    //            -- Pequeña optimización: solo actualizamos si el número realmente ha cambiado
    //            WHERE Orden<> NuevoOrden;
    //    END

    //        -- Confirmamos los cambios
    //        COMMIT TRAN;
    //    END TRY
    //    BEGIN CATCH
    //        --Si falla algo, marcha atrás
    //        IF @@TRANCOUNT > 0
    //            ROLLBACK TRAN;

    //    THROW;
    //    END CATCH
    //END
    //GO
    #endregion
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
        public async Task<ProgresoUsuario> VerProgresoUsuarioLastAsync(int idUsu, int idCur)
        {
            var ultimoProgreso = await this.context.ProgresoUsuario
                .Where(datos => datos.UsuarioID == idUsu && datos.CursoID == idCur)
                .OrderByDescending(datos => datos.FechaCompletado)
                .FirstOrDefaultAsync();
            return ultimoProgreso;
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
            if (await VerProgresoUsuarioAsync(idUsu, idLec) == null)
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
        public async Task EliminarLeccionEnCascada(int idLeccion)
        {
            string sql = "EXEC SP_EliminarLeccionEnCascada @LeccionID";
            SqlParameter pamId = new SqlParameter("@LeccionID", idLeccion);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamId);
        }

        public async Task UpdateLeccionAsync(Leccion leccionModificada)
        {
            Leccion leccionOriginal = await this.context.Leccion
                .FirstOrDefaultAsync(l => l.LeccionID == leccionModificada.LeccionID);

            if (leccionOriginal != null)
            {
                leccionOriginal.Titulo = leccionModificada.Titulo;
                leccionOriginal.ContenidoTeorico = leccionModificada.ContenidoTeorico;
                await this.context.SaveChangesAsync();
            }
        }
    }
}
