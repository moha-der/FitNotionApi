using System.ComponentModel.DataAnnotations;

namespace FitNotionApi.Models
{
    public class ComidasDieta
    {
        [Key]
        public int? id_comidasDieta { get; set; }
        public int? id_dieta { get; set; }
        public string opcion { get; set; }
        public string tipo_comida { get; set; }
        public string comida { get; set; }
        public string hora_comida { get; set; }
    }

    public class comidasDietaRequest
    {
        public string comida { get; set; }
        public string opcion { get; set; }
        public string alimento { get; set; }
        public string cantidad { get; set; }
        public string hora { get; set; }
    }

    public class DietaRequest
    {
        public string emailCliente { get; set; }
        public List<comidasDietaRequest> comidasDieta { get; set; }
    }
}
