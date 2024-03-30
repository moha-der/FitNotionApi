namespace FitNotionApi.Models
{
    public class RegistroComida
    {
        public Alimentos Alimento {  get; set; }
        public string Email { get; set; }
        public string TipoComida { get; set; }
        public DateTime Fecha { get; set; }
        public double Cantidad { get; set; }
        public double CaloriasTotal { get; set; }
        public double CaloriasObjetivo { get; set; }

    }
}
