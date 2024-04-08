using FitNotionApi.Context;
using FitNotionApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace FitNotionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DietasController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly Services.IAuthorizationService _authorizationService;

        public DietasController(AppDbContext context, Services.IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [Authorize]
        [HttpGet]
        [Route("getDietas")]
        public ActionResult<IEnumerable<DietasResponse>> getDietas()
        {
            var userEmailClaim = User.Claims.ToArray().FirstOrDefault().Value;

            var result = (from dt in _context.Dietas
                          join nt in _context.Nutricionistas on dt.Id_Nutricionista equals nt.Id_Nutricionista
                          join us in _context.Usuarios on nt.Id_Usuario equals us.Id_Usuario
                          where us.Email == userEmailClaim
                          select new DietasResponse
                          {
                              Id_Dieta = dt.Id_Dieta,
                              NombreCliente = us.Nombre + " " + us.Apellidos,
                              EmailCliente = us.Email,
                              Fecha = dt.Fec_creacion
                          }).ToList();

            return Ok(result);
        }
    }
}
