using FitNotionApi.Context;
using FitNotionApi.Models.Custom;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitNotionApi.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthorizationService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string GenerarToken(string idUser)
        {
            var key = _configuration.GetValue<string>("JwtSettings:Key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUser));
            claims.AddClaim(new Claim(ClaimTypes.Role, "Moha"));

            var credentialsToken = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = credentialsToken
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokenCreated = tokenHandler.WriteToken(tokenConfig);

            return tokenCreated;
        }

        public async Task<AuthorizationResponse> DevolverToken(AuthorizationRequest authorization)
        {
            var user_find = _context.Usuarios.FirstOrDefault(x =>
            x.Email == authorization.Email &&
            x.Password == authorization.Password);

            if (user_find == null)
            {
                return await Task.FromResult<AuthorizationResponse>(null);
            }

            string tokenCreated = GenerarToken(user_find.Id_Usuario.ToString());

            return new AuthorizationResponse() { Token = tokenCreated, Permiso = user_find.Tipo_Usuario, Msg = "OK", Result = true, email = user_find.Email };
        }
    }
}
