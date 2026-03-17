using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.Win32;
using NuGet.Protocol.Plugins;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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



    //    ALTER PROCEDURE SP_ObtenerProgresoMisCursos
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
    //        c.Imagen,
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




    //    ALTER VIEW VW_Admin_EstudiantesProgreso
    //    AS
    //SELECT
    //    u.UsuarioID,
    //    u.NombreUsuario,
    //    u.CorreoElectronico,
    //    u.Imagen,
    //    u.Rol,

    //    c.CursoID,
    //    c.Titulo AS NombreCurso,
    //    c.Descripcion,
    //    c.EtiquetaLenguaje,
    
    //    -- Cálculo del porcentaje a prueba de fallos
    //    CASE
    //        WHEN c.CursoID IS NULL THEN NULL -- El alumno no está inscrito en nada
    //        WHEN ISNULL(TotalLec.Total, 0) = 0 THEN 0 -- El curso aún no tiene lecciones
    //        ELSE CAST(ROUND((CAST(ISNULL(LecCompletadas.Cantidad, 0) AS FLOAT) / TotalLec.Total) * 100, 0) AS INT)
    //    END AS PorcentajeProgreso

    //FROM dbo.Usuarios u

    //-- LEFT JOIN para que salgan todos los usuarios, tengan o no cursos
    //LEFT JOIN dbo.CursosUsuario cu ON u.UsuarioID = cu.UsuarioID
    //LEFT JOIN dbo.Cursos c ON cu.CursoID = c.CursoID

    //-- Subconsulta 1: Total de lecciones de cada curso
    //LEFT JOIN (
    //    SELECT CursoID, COUNT(LeccionID) AS Total
    //    FROM dbo.Lecciones
    //    GROUP BY CursoID
    //) AS TotalLec ON c.CursoID = TotalLec.CursoID

    //-- Subconsulta 2: Lógica robusta importada del SP (buscando el CursoID a través de la tabla Lecciones)
    //LEFT JOIN(
    //    SELECT pu.UsuarioID, l.CursoID, COUNT(pu.LeccionID) AS Cantidad
    //    FROM dbo.ProgresoUsuario pu
    //    INNER JOIN dbo.Lecciones l ON pu.LeccionID = l.LeccionID
    //    GROUP BY pu.UsuarioID, l.CursoID
    //) AS LecCompletadas ON u.UsuarioID = LecCompletadas.UsuarioID AND c.CursoID = LecCompletadas.CursoID

    //--(Hemos eliminado el WHERE u.Rol = 1 para que esta vista sea 100 % reutilizable)
    //GO
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
        public async Task UpdateCursoAsync(Curso cursoModificado)
        {
            Curso cursoOriginal = await this.context.Curso.FindAsync(cursoModificado.CursoID);

            if (cursoOriginal != null)
            {
                cursoOriginal.Titulo = cursoModificado.Titulo;
                cursoOriginal.EtiquetaLenguaje = cursoModificado.EtiquetaLenguaje;
                cursoOriginal.Descripcion = cursoModificado.Descripcion;
                if (cursoModificado.Imagen != null && cursoModificado.Imagen.Length > 0)
                {
                    cursoOriginal.Imagen = cursoModificado.Imagen;
                }
                await this.context.SaveChangesAsync();
            }
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
        public async Task<int> GetPrimeraLeccionCursoAsync(int idCurso)
        {
            var primeraLeccion = await this.context.Leccion
                .Where(l => l.CursoID == idCurso)
                .OrderBy(l => l.Orden) // Ordenamos de menor a mayor
                .Select(l => l.LeccionID)  // Solo nos interesa traernos el número del ID, no toda la lección
                .FirstOrDefaultAsync();

            return primeraLeccion;
        }
        public async Task<int> GetSiguienteLeccionAsync(int idCurso, int idLeccionActual)
        {
            var siguienteLeccion = await this.context.Leccion
                .Where(l => l.CursoID == idCurso && l.LeccionID > idLeccionActual) 
                .OrderBy(l => l.Orden)
                .Select(l => l.LeccionID)
                .FirstOrDefaultAsync();

            return siguienteLeccion;
        }
        public async Task<List<EstudianteAdminVM>> VerEstudiantes()
        {
            var datosPlanos = await this.context.VistaEstudiantesCursos.ToListAsync();

            List<EstudianteAdminVM> listaEstudiantes = datosPlanos
                .GroupBy(d => d.UsuarioID)
                .Select(grupo => new EstudianteAdminVM
                {
                    UsuarioID = grupo.Key,
                    NombreUsuario = grupo.First().NombreUsuario,
                    CorreoElectronico = grupo.First().CorreoElectronico,
                    Imagen = grupo.First().Imagen,

                    CursosInscritos = grupo
                        .Where(x => x.CursoID != null)
                        .Select(x => new CursoProgresoVM
                        {
                            CursoID = (int)x.CursoID!,
                            Titulo = x.NombreCurso!,
                            PorcentajeProgreso = (int)x.PorcentajeProgreso!
                        }).ToList()
                })
                .ToList();

            return listaEstudiantes;
        }

    }
}
