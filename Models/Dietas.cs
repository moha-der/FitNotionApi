using System.ComponentModel.DataAnnotations;

namespace FitNotionApi.Models
{
    public class Dietas
    {
        [Key]
        public int? Id_Dieta { get; set; }

        public required string Id_Cliente { get; set; }

        public required string Id_Nutricionista { get; set; }

        public required DateTime Fec_creacion { get; set; }

        public string Activo {  get; set; }
    }

    public class DietasResponse
    {
        public int? Id_Dieta { get; set; }
        public string NombreCliente { get; set; }
        public string EmailCliente { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class ClienteResponse
    {
        public string emailCliente { get; set; }
        public string nombreCliente { get; set; }
    }

}
