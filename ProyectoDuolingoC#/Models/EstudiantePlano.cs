namespace ProyectoDuolingoC_.Models
{
    public class EstudiantePlano
    {
        public int UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public string CorreoElectronico { get; set; }
        public byte[]? Imagen { get; set; }
        public int Rol { get; set; }


        public int? CursoID { get; set; }
        public string? NombreCurso { get; set; }
        public string? Descripcion { get; set; } // ¡Nueva!
        public string? EtiquetaLenguaje { get; set; } // ¡Nueva!
        public int? PorcentajeProgreso { get; set; }
    }
}
