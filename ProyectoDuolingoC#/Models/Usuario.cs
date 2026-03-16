using ProyectoDuolingoC_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDuolingoC_.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("UsuarioID")]
        public int UsuarioID { get; set; }

        [Required]
        [Column("NombreUsuario")]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = null!;

        [Required]
        [Column("CorreoElectronico")]
        [StringLength(100)]
        public string CorreoElectronico { get; set; } = null!;

        [Column("ExperienciaTotal")]
        public int ExperienciaTotal { get; set; }

        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; }
        [Column("Rol")]
        public int Rol { get; set; }
        [Column("Imagen")]
        public byte[]? Imagen { get; set; }

        // Las propiedades de navegación NO llevan [Column]
        public virtual Autenticacion? Autenticacion { get; set; }
        public virtual List<ProgresoUsuario> ProgresoUsuarios { get; set; } = new List<ProgresoUsuario>();
        public virtual List<Ranking> Rankings { get; set; } = new List<Ranking>();
    }
}