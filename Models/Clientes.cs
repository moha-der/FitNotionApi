using System.ComponentModel.DataAnnotations;

namespace FitNotionApi.Models
{
    public class Clientes
    {
        [Key]
        public required string Id_Cliente { get; set; }

        public required string Id_Usuario { get; set; }

        public string Id_Nutricionista { get; set; }

        public string Confirmado { get; set; }
    }

    public class ClientesResponse
    {
        public required string Id_Cliente { get; set; }

        public required string Id_Usuario { get; set; }

        public string Id_Nutricionista { get; set; }

        public string Confirmado { get; set; }

        public string Nombre { get; set; }
    }
}
