namespace FitNotionApi.Models
{
    public class GetComidas
    {
        public string TipoComida { get; set; }
        public double? CaloriasTotal { get; set; }
        public List<AlimentoResponse> Alimentos { get; set; }
    }

    public class AlimentoResponse
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Calorias { get; set; }
        public double? Cantidad { get; set; }
    }

    public class ComidaRequest
    {
        public DateTime Fecha { get; set; }
        public string Email { get; set; }
    }
}
