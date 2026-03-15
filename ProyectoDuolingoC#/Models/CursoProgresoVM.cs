namespace ProyectoDuolingoC_.Models
{
    public class CursoProgresoVM
    {
            public int CursoID { get; set; }
            public string Titulo { get; set; } = null!;
            public string? Descripcion { get; set; }
            public string? EtiquetaLenguaje { get; set; }
            public byte[]? Imagen { get; set; } // O string?, según lo tengas

            // ¡Y aquí viene nuestro porcentaje, sin [NotMapped]!
            public int PorcentajeProgreso { get; set; }
        
    }
}

