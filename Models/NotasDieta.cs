using System.ComponentModel.DataAnnotations;

namespace FitNotionApi.Models
{
    public class NotasDieta
    {
        [Key]
        public int? id_nota { get; set; }
        public int? id_dieta { get; set; }
        public string? id_nutricionista { get; set; }
        public string? id_cliente { get; set; }
        public DateTime? fec_creacion { get; set; }
        public string desc_nota { get; set; }
    }

    public class NotaRequest
    {
        public int IdDieta { get; set; }
        public bool IsNutricionista { get; set; }
        public string Nota { get; set; }
    }
}
