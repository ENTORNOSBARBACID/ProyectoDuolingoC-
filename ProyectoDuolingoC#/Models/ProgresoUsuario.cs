using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDuolingoC_.Models
{
    [Table("ProgresoUsuario")]
    public class ProgresoUsuario
    {
        // Recuerda: Aún necesitas configurar la llave compuesta en el DbContext
        [Key, Column("UsuarioID", Order = 0)]
        public int UsuarioID { get; set; }

        [Key, Column("LeccionID", Order = 1)]
        public int LeccionID { get; set; }

        [Column("FechaCompletado")]

        public DateTime FechaCompletado { get; set; }
        [Column("CursoID")]
        public int? CursoID { get; set; }

        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; } = null!;

        [ForeignKey("LeccionID")]
        public virtual Leccion Leccion { get; set; } = null!;
    }
}