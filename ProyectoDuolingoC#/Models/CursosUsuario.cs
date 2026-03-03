using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDuolingoC_.Models
{
    [Table("CursosUsuario")]
    public class CursosUsuario
    {
        [Key, Column("UsuarioID", Order = 0)]
        public int UsuarioID { get; set; }

        [Key, Column("CursoID", Order = 1)]
        public int CursoID { get; set; }


        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; } = null!;

        [ForeignKey("CursoID")]
        public virtual Leccion Leccion { get; set; } = null!;
    }
}
