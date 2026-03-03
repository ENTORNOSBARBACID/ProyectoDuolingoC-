using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDuolingoC_.Models
{
    [Table("Ranking")]
    public class Ranking
    {
        [Key]
        [Column("RankingID")]
        public int RankingID { get; set; }

        [Column("UsuarioID")]
        public int UsuarioID { get; set; }

        [Column("Liga")]
        [StringLength(20)]
        public string Liga { get; set; } = "Bronce";

        [Column("PuntosSemanales")]
        public int PuntosSemanales { get; set; }

        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}