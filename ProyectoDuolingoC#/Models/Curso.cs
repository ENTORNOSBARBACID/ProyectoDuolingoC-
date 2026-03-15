using ProyectoDuolingoC_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDuolingoC_.Models
{
    [Table("Cursos")]
    public class Curso
    {
        [Key]
        [Column("CursoID")]
        public int CursoID { get; set; }

        [Required]
        [Column("Titulo")]
        [StringLength(100)]
        public string Titulo { get; set; } = null!;

        [Column("Descripcion")]
        public string? Descripcion { get; set; }

        [Column("EtiquetaLenguaje")]
        [StringLength(20)]
        public string? EtiquetaLenguaje { get; set; }

        [Column("Imagen")]
        public byte[]? Imagen { get; set; }
        [NotMapped] 
        public int PorcentajeProgreso { get; set; }

        public virtual ICollection<Leccion> Lecciones { get; set; } = new List<Leccion>();
    }
}