using System.ComponentModel.DataAnnotations;

namespace FitNotionApi.Models
{
    public class Usuarios
    {
        [Key]
        public required string Id_Usuario { get; set; }
        public required string Nombre { get; set; }
        public required string Apellidos { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public int Edad { get; set; }
        public DateTime Fecha_nac {  get; set; }
        public int Tipo_Usuario { get; set; }

        public void hashedPassword(string password)
        {
            this.Password = BCrypt.Net.BCrypt.HashPassword(password,10);
        }

        public bool verifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password,this.Password);
        }
    }
}
