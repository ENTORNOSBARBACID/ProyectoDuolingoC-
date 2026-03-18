using Humanizer;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Protocol.Plugins;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProyectoDuolingoC_.Repositories
{
    #region procedures
    //    CREATE PROCEDURE SP_EliminarLeccionEnCascada
    //    @LeccionID INT
    //AS
    //BEGIN
    //    SET NOCOUNT ON;

    //    -- 1. Capturamos el CursoID antes de borrar para el reordenamiento posterior
    //    DECLARE @CursoID INT;
    //    SELECT @CursoID = CursoID FROM dbo.Lecciones WHERE LeccionID = @LeccionID;

    //    BEGIN TRY
    //        BEGIN TRANSACTION;

    //        -- 2. DESVINCULAR OPCIÓN CORRECTA(Obligatorio para evitar error de FK)
    //        -- Si la pregunta tiene un OpcionCorrectaID, lo ponemos a NULL
    //        -- Así la tabla OpcionesRespuesta queda libre para ser borrada
    //        UPDATE dbo.Preguntas
    //        SET OpcionCorrectaID = NULL
    //        WHERE LeccionID = @LeccionID AND OpcionCorrectaID IS NOT NULL;

    //        -- 3. BORRAR FÍSICAMENTE LAS OPCIONES(Nietos)
    //        -- Ahora que nada las apunta desde Preguntas, las eliminamos
    //        DELETE FROM dbo.OpcionesRespuesta
    //        WHERE PreguntaID IN(
    //            SELECT PreguntaID FROM dbo.Preguntas WHERE LeccionID = @LeccionID
    //        );

    //        -- 4. BORRAR PREGUNTAS(Hijos)
    //        DELETE FROM dbo.Preguntas
    //        WHERE LeccionID = @LeccionID;

    //        -- 5. BORRAR PROGRESO(Hijos)
    //        DELETE FROM dbo.ProgresoUsuario
    //        WHERE LeccionID = @LeccionID;

    //        -- 6. BORRAR LA LECCIÓN(Padre)
    //        DELETE FROM dbo.Lecciones
    //        WHERE LeccionID = @LeccionID;

    //        -- 7. REORDENAMIENTO DE LECCIONES
    //        IF @CursoID IS NOT NULL
    //        BEGIN
    //            WITH LeccionesReordenadas AS(
    //                SELECT
    //                    LeccionID,
    //                    ROW_NUMBER() OVER(ORDER BY Orden ASC, LeccionID ASC) AS NuevoOrden
    //                FROM dbo.Lecciones
    //                WHERE CursoID = @CursoID
    //            )
    //            UPDATE L
    //            SET L.Orden = LR.NuevoOrden
    //            FROM dbo.Lecciones L
    //            JOIN LeccionesReordenadas LR ON L.LeccionID = LR.LeccionID
    //            WHERE L.Orden<> LR.NuevoOrden;
    //    END

    //    COMMIT TRANSACTION;
    //    END TRY
    //    BEGIN CATCH
    //        IF @@TRANCOUNT > 0
    //            ROLLBACK TRANSACTION;

    //        -- Lanzamos el error para que C# se entere de qué ha pasado
    //        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
    //    DECLARE @Sev INT = ERROR_SEVERITY();
    //    DECLARE @St INT = ERROR_STATE();
    //    RAISERROR(@Msg, @Sev, @St);
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
