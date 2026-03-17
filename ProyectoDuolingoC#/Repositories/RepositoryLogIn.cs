using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcCoreCryptography.Helpers;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Models;
using System.Data.SqlTypes;

namespace ProyectoDuolingoC_.Repositories
{
    public class RepositoryLogIn
    {
        private ProyectoContext context;
        public RepositoryLogIn(ProyectoContext context)
        {
            this.context = context;
        }
        public async Task RegisterUsuario(string nombre, string email, byte[] imagen, int rol, string password)
        {
            Usuario use = new Usuario();
            use.NombreUsuario = nombre;
            use.CorreoElectronico = email;
            use.Imagen = imagen;
            use.ExperienciaTotal = 0;
            use.Rol = rol;
            use.FechaRegistro = DateTime.Now;


            string Salt = HelperTools.GenerarSalt();
            byte[] Password = HelperCryptography.EncryptPassword(password, Salt);
            Autenticacion au = new Autenticacion();
            au.UsuarioID = use.UsuarioID;
            au.PasswordHash = Password;
            au.Salt = Salt;

            use.Autenticacion = au;
            au.Usuario = use;
            await this.context.Usuario.AddAsync(use);
            await this.context.SaveChangesAsync();
        }
        public async Task<Usuario> LogInUserAsync(string email, string password)
        {
            Usuario user = await this.context.Usuario
                             .Include(u => u.Autenticacion)
                             .FirstOrDefaultAsync(u => u.CorreoElectronico == email);
            if (user == null)
            {
                return null;
            }
            else
            {
                string salt = user.Autenticacion.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                byte[] passbytes = user.Autenticacion.PasswordHash;
                bool response = HelperTools.CompareArrays(temp, passbytes);
                if (response == true)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }
        public async Task<Usuario> FindUsuarioByIDAsync(int id)
        {
            Usuario user = await this.context.Usuario
                             .Include(u => u.Autenticacion)
                             .FirstOrDefaultAsync(u => u.UsuarioID == id);
            return user;
        }

        public async Task UpdatePerfilAsync(int idUsuario, string nuevoNombre, byte[] nuevaImagen)
        {
            Usuario userOriginal = await this.context.Usuario.FindAsync(idUsuario);

            if (userOriginal != null)
            {
                userOriginal.NombreUsuario = nuevoNombre;

                if (nuevaImagen != null && nuevaImagen.Length > 0)
                {
                    userOriginal.Imagen = nuevaImagen;
                }

                await this.context.SaveChangesAsync();
            }
        }

        public async Task<List<Usuario>> ObtenerRankingGlobalAsync()
        {
            return await this.context.Usuario
                .Where(u => u.Rol == 1)
                .OrderByDescending(u => u.ExperienciaTotal)
                .ToListAsync();
        }
    }
}
