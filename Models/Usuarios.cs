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
    }
}
