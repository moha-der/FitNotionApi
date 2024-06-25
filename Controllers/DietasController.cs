using Azure.Core;
using FitNotionApi.Context;
using FitNotionApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                          join cl in _context.Clientes on dt.Id_Cliente equals cl.Id_Cliente
                          join us2 in _context.Usuarios on cl.Id_Usuario equals us2.Id_Usuario
                          where us.Email == userEmailClaim && dt.Activo == "S"
                          select new DietasResponse
                          {
                              Id_Dieta = dt.Id_Dieta,
                              NombreCliente = us2.Nombre + " " + us2.Apellidos,
                              EmailCliente = us2.Email,
                              Fecha = dt.Fec_creacion
                          }).ToList();

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        [Route("getCliente")]
        public ActionResult<IEnumerable<ClienteResponse>> getCliente([FromQuery] string emailCliente)
        {

            var clientes = (from us in _context.Usuarios
                            join cl in _context.Clientes on us.Id_Usuario equals cl.Id_Usuario 
                            where us.Email.Contains(emailCliente) && us.Tipo_Usuario == 1 && cl.Id_Nutricionista == null
                            select new ClienteResponse
                            {
                                emailCliente = us.Email,
                                nombreCliente = us.Nombre + " " + us.Apellidos
                            }).ToList();

            if (clientes == null)
            {
                return Ok("Sin Clientes");
            }

            return Ok(clientes);
        }

        [Authorize]
        [HttpGet]
        [Route("getClientesAsignados")]
        public ActionResult<IEnumerable<ClienteResponse>> getClienteAsignados()
        {
            var userEmailClaim = User.Claims.ToArray().FirstOrDefault().Value;

            var resultado = (from cl in _context.Clientes
                             join nt in _context.Nutricionistas on cl.Id_Nutricionista equals nt.Id_Nutricionista
                             join us in _context.Usuarios on nt.Id_Usuario equals us.Id_Usuario
                             join us2 in _context.Usuarios on cl.Id_Usuario equals us2.Id_Usuario
                             where us.Email == userEmailClaim
                             select new ClienteResponse
                             {
                                 emailCliente = us2.Email,
                                 nombreCliente = us2.Nombre + " " + us2.Apellidos
                             }
                             ).ToList();

            if (resultado == null)
            {
                return Ok("Sin Clientes");
            }

            return Ok(resultado);
        }

        [Authorize]
        [HttpPut]
        [Route(("AsignarCliente/{email}"))]
        public IActionResult addCliente(string email)
        {
            var userEmailClaim = User.Claims.ToArray().FirstOrDefault().Value;

            var cliente = (from us in _context.Usuarios
                           join cl in _context.Clientes on us.Id_Usuario equals cl.Id_Usuario
                           where us.Email == email
                           select cl).FirstOrDefault();

            if (cliente == null)
            {
                return Ok("ERROR | Cliente no encontrado");
            }

            var nutricionista = (from us in _context.Usuarios
                                 join nt in _context.Nutricionistas on us.Id_Usuario equals nt.Id_Usuario
                                 where us.Email == userEmailClaim
                                 select new Nutricionistas
                                 {
                                     Id_Usuario = us.Id_Usuario,
                                     Id_Nutricionista = nt.Id_Nutricionista,
                                 }).FirstOrDefault();

            cliente.Id_Nutricionista = nutricionista.Id_Nutricionista;
            _context.SaveChanges();

            return Ok("OK | Cliente añadido");
        }

        [Authorize]
        [HttpDelete]
        [Route(("DesAsignarCliente/{email}"))]
        public IActionResult deleteCliente(string email)
        {
            var userEmailClaim = User.Claims.ToArray().FirstOrDefault().Value;

            var cliente = (from us in _context.Usuarios
                           join cl in _context.Clientes on us.Id_Usuario equals cl.Id_Usuario
                           where us.Email == email
                           select cl).FirstOrDefault();

            if (cliente == null)
            {
                return Ok("ERROR | Cliente no encontrado");
            }

            

            cliente.Id_Nutricionista = null;
            _context.SaveChanges();

            return Ok("OK | Cliente borrado");
        }


        [HttpGet]
        [Route(("DetalleDieta/{idDieta}"))]
        public async Task<ActionResult> detalleDietas(int idDieta)
        {
            var comidasDieta = await _context.ComidasDieta
                                         .Where(cd => cd.id_dieta == idDieta)
                                         .ToListAsync();

            var data = new
            {
                desayuno = GetComidaOptions(comidasDieta, "Desayuno"),
                almuerzo = GetComidaOptions(comidasDieta, "Almuerzo"),
                comida = GetComidaOptions(comidasDieta, "Comida"),
                merienda = GetComidaOptions(comidasDieta, "Merienda"),
                cena = GetComidaOptions(comidasDieta, "Cena"),
                notas = new
                {
                    nota = new
                    {
                        usuario = "moha",
                        nota = "hola mundo"
                    }
                }
            };

            return Ok(data);

        }

        private object GetComidaOptions(IEnumerable<ComidasDieta> comidasDieta, string tipoComida)
        {
            var comidasFiltradas = comidasDieta.Where(cd => cd.tipo_comida == tipoComida).ToList();

            if (comidasFiltradas.Any())
            {
                string horaComida = comidasFiltradas.First().hora_comida;

                string opcionA = "";
                string opcionB = "";
                string opcionC = "";

                foreach (var comida in comidasFiltradas)
                {
                    switch (comida.opcion)
                    {
                        case "A":
                            opcionA = comida.comida;
                            break;
                        case "B":
                            opcionB = comida.comida;
                            break;
                        case "C":
                            opcionC = comida.comida;
                            break;
                        default:
                            break;
                    }
                }

                return new
                {
                    opciones = new
                    {
                        opcionA,
                        opcionB,
                        opcionC
                    },
                    hora = horaComida
                };
            }

            return new
            {
                opciones = new
                {
                    opcionA = "",
                    opcionB = "",
                    opcionC = ""
                },
                hora = ""
            };
        }



        private object GetComidasDetalle(ComidasDieta comidaDieta)
        {
            return new
            {
                opcionA = comidaDieta.opcion == "A" ? comidaDieta.comida : null,
                opcionB = comidaDieta.opcion == "B" ? comidaDieta.comida : null,
                opcionC = comidaDieta.opcion == "C" ? comidaDieta.comida : null
            };
        }


        [HttpPost]
        [Route(("AddDieta"))]
        public async Task<IActionResult> AddDieta([FromBody] DietaRequest request)
        {

            var userEmailClaim = User.Claims.ToArray().FirstOrDefault().Value;

            var nutricionista = (from us in _context.Usuarios
                                 join nt in _context.Nutricionistas on us.Id_Usuario equals nt.Id_Usuario
                                 where us.Email == userEmailClaim
                                 select new Nutricionistas
                                 {
                                     Id_Usuario = us.Id_Usuario,
                                     Id_Nutricionista = nt.Id_Nutricionista,
                                 }).FirstOrDefault();

            if (nutricionista == null)
            {
                return Ok("ERROR | Nutricionista no encontrado");
            }

            var cliente = (from us in _context.Usuarios
                           join cl in _context.Clientes on us.Id_Usuario equals cl.Id_Usuario
                           where us.Email == request.emailCliente
                           select cl).FirstOrDefault();

            if (cliente == null)
            {
                return Ok("ERROR | Cliente no encontrado");
            }

            var nuevaDieta = new Dietas
            {
                Id_Cliente = cliente.Id_Cliente,
                Id_Nutricionista = nutricionista.Id_Nutricionista,
                Fec_creacion = DateTime.Now,
                Activo = "S"
            };

            _context.Dietas.Add(nuevaDieta);
            await _context.SaveChangesAsync();

            foreach(var comida in request.comidasDieta)
            {
                var nuevaComidaDieta = new ComidasDieta
                {
                    id_dieta = nuevaDieta.Id_Dieta,
                    opcion = comida.opcion,
                    tipo_comida = comida.comida,
                    comida = comida.alimento,
                    hora_comida = comida.hora
                };

                _context.ComidasDieta.Add(nuevaComidaDieta);
            }

            await _context.SaveChangesAsync();

            return Ok("Dieta y comida guardados correctamente");
            

        }
    }
}
