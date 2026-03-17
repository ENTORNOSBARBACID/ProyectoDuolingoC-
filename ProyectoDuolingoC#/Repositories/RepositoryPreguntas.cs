using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;

namespace ProyectoDuolingoC_.Repositories
{
    #region Procedures
    //    CREATE PROCEDURE SP_EliminarPregunta
    //    @PreguntaID INT
    //AS
    //BEGIN
    //    SET NOCOUNT ON;
    
    //    -- 1. Primero, averiguamos si la pregunta tiene una "OpcionCorrectaID" asociada
    //    DECLARE @OpcionCorrecta INT;
    //    SELECT @OpcionCorrecta = OpcionCorrectaID
    //    FROM dbo.Preguntas
    //    WHERE PreguntaID = @PreguntaID;

    //    BEGIN TRY
    //        BEGIN TRAN;

    //        -- 2. SI TIENE OPCIONES(No es nulo), procedemos a limpiarlas primero
    //        IF @OpcionCorrecta IS NOT NULL
    //        BEGIN
    //            --Rompemos la atadura(Referencia Circular) para que nos deje borrar
    //            UPDATE dbo.Preguntas
    //            SET OpcionCorrectaID = NULL
    //            WHERE PreguntaID = @PreguntaID;

    //            -- Borramos todas las respuestas de la tabla OpcionesRespuesta atadas a esta pregunta
    //            DELETE FROM dbo.OpcionesRespuesta
    //            WHERE PreguntaID = @PreguntaID;
    //    END

    //        -- 3. Finalmente(tenga o no tenga opciones previas), borramos la Pregunta
    //        DELETE FROM dbo.Preguntas
    //        WHERE PreguntaID = @PreguntaID;

    //        -- Si todo es correcto, guardamos los cambios en firme
    //        COMMIT TRAN;
    //    END TRY
    //    BEGIN CATCH
    //        --Si hay error, marcha atrás
    //        IF @@TRANCOUNT > 0
    //            ROLLBACK TRAN;

    //    THROW;
    //    END CATCH
    //END
    //GO
    #endregion
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
        public async Task EliminarPregunta(int id)
        {
            string sql = "EXEC SP_EliminarPregunta @preguntaID";
            SqlParameter pamId = new SqlParameter("@preguntaID", id);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamId);
        }

        public async Task SumarPuntos(int puntos, int idUsuario)
        {
            var usuario = await this.context.Usuario
                .FirstOrDefaultAsync(u => u.UsuarioID == idUsuario);

            if (usuario != null)
            {
                usuario.ExperienciaTotal = (usuario.ExperienciaTotal) + puntos;
                await this.context.SaveChangesAsync();
            }
        }
    }
}
