using System.ComponentModel.DataAnnotations;

namespace FitNotionApi.Models
{
    public class ConsumoDiario
    {
        public int? Id { get; set; }

        public required string Id_Usuario { get; set; }

        public required DateTime Fecha { get; set; }
    }
}
