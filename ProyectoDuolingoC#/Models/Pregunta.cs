using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDuolingoC_.Models
{
    [Table("Preguntas")]
    public class Pregunta
    {
        [Key]
        [Column("PreguntaID")]
        public int PreguntaID { get; set; }

        [Column("LeccionID")]
        public int LeccionID { get; set; }

        [Required]
        [Column("TextoPregunta")]
        public string TextoPregunta { get; set; } = null!;

        [Column("TipoPregunta")]
        [StringLength(30)]
        public string? TipoPregunta { get; set; }

        [Required]
        [Column("RespuestaCorrecta")]
        public string RespuestaCorrecta { get; set; } = null!;

        [Column("PuntosXP")]
        public int PuntosXP { get; set; }

        [ForeignKey("LeccionID")]
        public virtual Leccion Leccion { get; set; } = null!;
    }
}