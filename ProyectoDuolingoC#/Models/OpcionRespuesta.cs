using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDuolingoC_.Models
{
    [Table("OpcionesRespuesta")]
    public class OpcionRespuesta
    {
        [Key]
        [Column("OpcionID")]
        public int OpcionID { get; set; }

        [Column("PreguntaID")]
        public int PreguntaID { get; set; }

        [Required]
        [Column("TextoOpcion")]
        public string TextoOpcion { get; set; } = null!;

        [ForeignKey("PreguntaID")]
        public virtual Pregunta Pregunta { get; set; } = null!;
    }
}