using System.ComponentModel.DataAnnotations;

namespace FitNotionApi.Models
{
    public class Nutricionistas
    {
        [Key]
        public required string Id_Nutricionista { get; set; }

        public required string Id_Usuario { get; set; }
        
        public string? Nombre { get; set; }
    }
}
