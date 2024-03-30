using FitNotionApi.Context;
using FitNotionApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FitNotionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlimentosController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly Services.IAuthorizationService _authorizationService;

        public AlimentosController(AppDbContext context, Services.IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }


        [HttpGet]
        [Route("getAlimentosDia")]
        public ActionResult<IEnumerable<GetComidas>> getAlimentosDia([FromQuery] string email, [FromQuery] string fecha)
        {
            DateTime fechaDateTime = DateTime.ParseExact(fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var userEmailClaim = User.Claims.FirstOrDefault(c => c.Value == "email");


            var user_find = _context.Usuarios.FirstOrDefault(
                x => x.Email == email);

            if (user_find == null)
            {
                return BadRequest("Usuario No existe");
            }

            var comidas = (from cs in _context.ConsumoDiario
                             join cd in _context.ConsumoDetalle on cs.Id equals cd.Id_consumo_diario
                             join al in _context.Alimentos on cd.Id_alimento equals al.Id
                             join us in _context.Usuarios on cs.Id_Usuario equals us.Id_Usuario
                             where us.Email == email
                             && cs.Fecha.Date == fechaDateTime.Date 
                             group new { al, cd } by cd.Tipo_comida into g
                             select new GetComidas
                             {
                                 TipoComida = g.Key,
                                 CaloriasTotal = g.Sum(x => x.al.Calorias * x.cd.Cantidad),
                                 Alimentos = g.Select(x => new AlimentoResponse
                                 {
                                     Id = x.al.Id.ToString(),
                                     Nombre = x.al.Nombre,
                                     Calorias = x.al.Calorias.ToString(),
                                     Cantidad = x.cd.Cantidad
                                 }).ToList()
                             }).ToList();


            var queryTotal = (from cd in _context.ConsumoDetalle
                                           join al in _context.Alimentos on cd.Id_alimento equals al.Id
                                           join cs in _context.ConsumoDiario on cd.Id_consumo_diario equals cs.Id
                                           where cs.Id_Usuario == user_find.Id_Usuario && cs.Fecha.Date == fechaDateTime.Date
                                           select new
                                           {
                                               Calorias = cd.Cantidad * al.Calorias,
                                               Proteinas = cd.Cantidad * al.Proteinas,
                                               Carbohidratos = cd.Cantidad * al.Carbohidratos,
                                               Grasas = cd.Cantidad * al.Grasas,
                                               Fibra = cd.Cantidad * al.Fibra
                                           });

            decimal caloriasTotalConsumidas = (decimal) queryTotal.Sum(x => x.Calorias);
            decimal proteinasTotalConsumidas = (decimal) queryTotal.Sum(x => x.Proteinas);
            decimal hidratosTotalConsumidos = (decimal) queryTotal.Sum(x => x.Carbohidratos);
            decimal grasasTotalConsumidas = (decimal)queryTotal.Sum(x => x.Grasas);
            decimal fibrasTotalConsumidas = (decimal)queryTotal.Sum(x => x.Fibra);




            var resultado = new
            {
                caloriasObjetivo = 1000,
                caloriasConsumidas = caloriasTotalConsumidas,
                proteinasConsumidas = proteinasTotalConsumidas,
                hidratosConsumidos = hidratosTotalConsumidos,
                grasasConsumidas = grasasTotalConsumidas,
                fibrasConsumidas = fibrasTotalConsumidas,
                detalleComidas = comidas
            };

            return Ok(resultado);
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarAlimento([FromBody] RegistroComida registro)
        {
            string email = registro.Email;
            Alimentos alimento = registro.Alimento;
            DateTime fecha = registro.Fecha;
            string tipoComida = registro.TipoComida;
            double cantidad = registro.Cantidad;
            //Buscamos el id_usuario 
            var user_find = _context.Usuarios.FirstOrDefault(x =>
            x.Email == email);

            if (user_find == null)
            {
                return BadRequest("Usuario No existe");
            }

            var consumo_find = _context.ConsumoDiario.FirstOrDefault(
                x => x.Id_Usuario == user_find.Id_Usuario && 
                x.Fecha.Date == fecha.Date);

            if (consumo_find == null)
            {
                ConsumoDiario nuevoConsumo = new ConsumoDiario 
                { 
                    Fecha = fecha, 
                    Id_Usuario = user_find.Id_Usuario 
                };
                _context.ConsumoDiario.Add(nuevoConsumo);
                _context.SaveChanges();

                consumo_find = nuevoConsumo;
            }

            var alimento_find = _context.Alimentos.FirstOrDefault(
                x => x.Id == alimento.Id);

            if (alimento_find == null)
            {
                Alimentos nuevoAlimento = new Alimentos
                {
                    Id = alimento.Id,
                    Nombre = alimento.Nombre,
                    Calorias = alimento.Calorias,
                    Carbohidratos = alimento.Carbohidratos,
                    Proteinas = alimento.Proteinas,
                    Fibra = alimento.Fibra,
                    Grasas = alimento.Grasas,
                    Racion = alimento.Racion,
                };

                _context.Alimentos.Add(nuevoAlimento);
                _context.SaveChanges();

                alimento_find = nuevoAlimento;
            }

            var ConsumoDetalle_find = _context.ConsumoDetalle.FirstOrDefault(
                x => x.Id_consumo_diario == consumo_find.Id && x.Tipo_comida == tipoComida && x.Id_alimento == alimento_find.Id);

            if (ConsumoDetalle_find != null)
            {
                ConsumoDetalle_find.Cantidad += cantidad;
                await _context.SaveChangesAsync();

                return Ok();
            }

            ConsumoDetalle nuevoDetalle = new ConsumoDetalle
            {
                Id_alimento = alimento_find.Id,
                Tipo_comida = tipoComida,
                Cantidad = cantidad,
                Id_consumo_diario = (int) consumo_find.Id
            };

            _context.ConsumoDetalle.Add(nuevoDetalle);
            _context.SaveChanges();
            return Ok();
        }
    }
}

