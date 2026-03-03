using ProyectoDuolingoC_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDuolingoC_.Models
{
    [Table("Lecciones")]
    public class Leccion
    {
        [Key]
        [Column("LeccionID")]
        public int LeccionID { get; set; }

        [Column("CursoID")]
        public int CursoID { get; set; }

        [Required]
        [Column("Titulo")]
        [StringLength(100)]
        public string Titulo { get; set; } = null!;

        [Column("Orden")]
        public int Orden { get; set; }

        [ForeignKey("CursoID")]
        public virtual Curso Curso { get; set; } = null!;

        public virtual ICollection<Pregunta> Preguntas { get; set; } = new List<Pregunta>();
        public virtual ICollection<ProgresoUsuario> ProgresoUsuarios { get; set; } = new List<ProgresoUsuario>();
    }
}