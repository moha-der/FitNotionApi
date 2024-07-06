using FitNotionApi.Context;
using FitNotionApi.Models.Custom;
using FitNotionApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;


namespace FitNotionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly Services.IAuthorizationService _authorizationService;
        
        private readonly IEmailService _emailService;

        public AccountController(AppDbContext context, Services.IAuthorizationService authorizationService, IEmailService emailService)
        {
            _context = context;
            _authorizationService = authorizationService;
            _emailService = emailService;
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
            string password = nuevoUsuario.Password;

            nuevoUsuario.Id_Usuario = Guid.NewGuid().ToString();
            nuevoUsuario.hashedPassword(password);


            if (user_find != null)
            {
                return BadRequest("Error al registrar el usuario");
            }

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();
            if (nuevoUsuario.Tipo_Usuario == 2)
            {
                Nutricionistas nutricionista = new Nutricionistas
                {
                    Id_Nutricionista = Guid.NewGuid().ToString(),
                    Id_Usuario = nuevoUsuario.Id_Usuario
                };
                _context.Nutricionistas.Add(nutricionista);
            } else
            {
                Clientes cliente = new Clientes
                {
                    Id_Cliente = Guid.NewGuid().ToString(),
                    Id_Usuario = nuevoUsuario.Id_Usuario,
                };
                _context.Clientes.Add(cliente);
            }
            
            _context.SaveChanges();

            AuthorizationRequest authorization = new AuthorizationRequest();
            authorization.Email = nuevoUsuario.Email;
            authorization.Password = password;

            var authorization_result = await _authorizationService.DevolverToken(authorization);

            if (authorization_result == null)
            {
                return Unauthorized();
            }

            return Ok(authorization_result);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.Claims.ToArray().FirstOrDefault().Value;

            if (email == null)
            {
                return Unauthorized();
            }

            var user = await _context.Usuarios
            .Where(u => u.Email == email)
            .Select(u => new
            {
                u.Nombre,
                u.Apellidos,
                u.Email,
                FechaNac = u.Fecha_nac.ToString("yyyy-MM-dd")
            })
            .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPost("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfile userUpdateDto)
        {
            var email = User.Claims.ToArray().FirstOrDefault().Value;

            if (email == null)
            {
                return Unauthorized();
            }

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            user.Nombre = userUpdateDto.Nombre;
            user.Apellidos = userUpdateDto.Apellidos;
            user.Fecha_nac = DateTime.Parse(userUpdateDto.FechaNac);

            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                return BadRequest("Las contraseñas no coinciden");
            }

            var email = User.Claims.ToArray().FirstOrDefault().Value;

            if (email == null)
            {
                return Unauthorized();
            }

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if(!user.verifyPassword(changePasswordDto.CurrentPassword))
            {
                return BadRequest("Las contraseñas no coinciden");
            }

            user.Password = changePasswordDto.NewPassword;
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();
            return Ok("Password changed successfully.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);
            if (user == null)
            {
                return Ok(); 
            }

            var token = GenerateResetToken();
            user.Recover_token = token;
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            var resetLink = $"https://fit-notion.vercel.app/resetPassword?token={token}";
            var emailMessage = $"Para reestablecer tu contraseña click aquí: <a href='{resetLink}'>link</a>";

            await _emailService.SendEmailAsync(user.Email, "Resetear Contraseña", emailMessage);

            return Ok();
        }

        private string GenerateResetToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Recover_token == resetPasswordDto.Token);
            if (user == null)
            {
                return BadRequest("Token Inválido.");
            }

            user.hashedPassword(resetPasswordDto.NewPassword);
            user.Recover_token = "";
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Contraseña cambiada correctamente");
        }

    }
}
