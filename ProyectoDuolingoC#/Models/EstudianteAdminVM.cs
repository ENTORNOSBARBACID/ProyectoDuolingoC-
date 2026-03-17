namespace ProyectoDuolingoC_.Models
{
    public class EstudianteAdminVM
    {
        public int UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public string CorreoElectronico { get; set; }
        public byte[]? Imagen { get; set; }
        public int Rol { get; set; } 

        // Aquí metemos la lista de cursos que ya usábamos antes
        public List<CursoProgresoVM> CursosInscritos { get; set; } = new List<CursoProgresoVM>();
    }
}
