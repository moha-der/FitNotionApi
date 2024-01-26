namespace FitNotionApi.Models.Custom
{
    public class AuthorizationResponse
    {
        public string Token { get; set; }
        public bool Result { get; set; }
        public string Msg { get; set; }
        public int Permiso { get; set; }
        public string email { get; set; }
    }
}
