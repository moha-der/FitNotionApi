using System.ComponentModel.DataAnnotations;

namespace FitNotionApi.Models
{
    public class Alimentos
    {
        [Key]
        public required string Id { get; set; }

        public required string Nombre { get; set; }

        public double? Proteinas { get; set; }

        public double? Carbohidratos { get; set; }

        public double? Grasas { get; set; }

        public double? Calorias { get; set; }

        public double? Fibra { get; set; }

        public double Racion { get; set; }
    }
}
