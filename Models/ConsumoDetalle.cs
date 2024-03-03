using System.ComponentModel.DataAnnotations;

namespace FitNotionApi.Models
{
    public class ConsumoDetalle
    {
        public int? Id { get; set; }

        public required int Id_consumo_diario { get; set; }

        public required string Id_alimento { get; set; }

        public required string Tipo_comida { get; set; }

        public double? Cantidad { get; set; }
    }
}
