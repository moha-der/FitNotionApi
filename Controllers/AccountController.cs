using FitNotionApi.Context;
using FitNotionApi.Models.Custom;
using FitNotionApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitNotionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly Services.IAuthorizationService _authorizationService;

        public AccountController(AppDbContext context, Services.IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] AuthorizationRequest authorization)
        {

            var authorization_result = await _authorizationService.DevolverToken(authorization);
            
            if (authorization_result == null)
            {
                return Unauthorized();
            }

            return Ok(authorization_result);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] Usuarios nuevoUsuario)
        {
            var user_find = _context.Usuarios.FirstOrDefault(x =>
            x.Email == nuevoUsuario.Email);

            nuevoUsuario.Id_Usuario = Guid.NewGuid().ToString();


            if (user_find != null)
            {
                return BadRequest("Error al registrar el usuario");
            }

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            AuthorizationRequest authorization = new AuthorizationRequest();
            authorization.Email = nuevoUsuario.Email;
            authorization.Password = nuevoUsuario.Password;

            var authorization_result = await _authorizationService.DevolverToken(authorization);

            if (authorization_result == null)
            {
                return Unauthorized();
            }

            return Ok(authorization_result);
        }
    }
}
