using Microsoft.EntityFrameworkCore;
using ProyectoDuolingoC_.Models;
using System.Collections.Generic;

namespace ProyectoDuolingoC_.Data
{
    public class ProyectoContext:DbContext
    {
        public ProyectoContext(DbContextOptions<ProyectoContext> options) : base(options) { }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<Autenticacion> Autenticacion { get; set; }
        public DbSet<Leccion> Leccion { get; set; }
        public DbSet<Pregunta> Pregunta { get; set; }
        public DbSet<ProgresoUsuario> ProgresoUsuario { get; set; }
        public DbSet<CursosUsuario> CursosUsuarios { get; set; }
        public DbSet<Ranking> Ranking { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Tu corrección de la clave compuesta (ESTO YA ESTÁ BIEN)
            modelBuilder.Entity<ProgresoUsuario>()
                .HasKey(pu => new { pu.UsuarioID, pu.LeccionID });
            modelBuilder.Entity<CursosUsuario>()
                .HasKey(pu => new { pu.UsuarioID, pu.CursoID });

            // 2. Mapeo de nombres de tablas (SOLO SI TE DA ERROR DE "INVALID OBJECT NAME")
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Curso>().ToTable("Cursos");
            modelBuilder.Entity<Autenticacion>().ToTable("Autenticacion"); // Esta estaba en singular en tu SQL
            modelBuilder.Entity<Leccion>().ToTable("Lecciones");
            modelBuilder.Entity<Pregunta>().ToTable("Preguntas");
            modelBuilder.Entity<Ranking>().ToTable("Ranking"); // Esta estaba en singular en tu SQL

            base.OnModelCreating(modelBuilder);
        }
    }
}
