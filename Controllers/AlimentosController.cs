using FitNotionApi.Context;
using FitNotionApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                x => x.Id_Usuario == user_find.Id_Usuario);

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

