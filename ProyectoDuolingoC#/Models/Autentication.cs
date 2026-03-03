using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDuolingoC_.Models
{
    [Table("Autenticacion")]
    public class Autenticacion
    {
        [Key]
        [Column("UsuarioID")]
        [ForeignKey("Usuario")]
        public int UsuarioID { get; set; }

        [Required]
        [Column("PasswordHash")]
        public byte[] PasswordHash { get; set; } = null!;

        [Required]
        [Column("Salt")]
        public string Salt { get; set; } = null!;

        public virtual Usuario Usuario { get; set; } = null!;
    }
}