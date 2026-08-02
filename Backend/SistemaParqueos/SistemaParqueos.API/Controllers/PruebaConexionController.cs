using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaParqueos.AccesoDatos.Contexto;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PruebaConexionController : ControllerBase
{
    private readonly ParqueosDbContext _contexto;

    public PruebaConexionController(
        ParqueosDbContext contexto)
    {
        _contexto = contexto;
    }

    [HttpGet]
    public async Task<IActionResult> ProbarConexion()
    {
        try
        {
            var puedeConectarse =
                await _contexto.Database.CanConnectAsync();

            if (!puedeConectarse)
            {
                var respuestaError =
                    Respuesta.Fallida(
                        "No fue posible conectarse a ParqueosDB.",
                        500);

                return StatusCode(
                    respuestaError.CodigoEstado,
                    respuestaError);
            }

            var cantidadClientes =
                await _contexto.Clientes.CountAsync();

            var cantidadVehiculos =
                await _contexto.Vehiculos.CountAsync();

            var cantidadParqueos =
                await _contexto.Parqueos.CountAsync();

            var datos = new
            {
                BaseDatos = "ParqueosDB",
                CantidadClientes = cantidadClientes,
                CantidadVehiculos = cantidadVehiculos,
                CantidadParqueos = cantidadParqueos
            };

            var respuesta =
                Respuesta.Correcta(
                    datos,
                    "Conexión exitosa con ParqueosDB.");

            return StatusCode(
                respuesta.CodigoEstado,
                respuesta);
        }
        catch (Exception ex)
        {
            var respuesta =
                Respuesta.Fallida(
                    "Ocurrió un error al probar la conexión.",
                    500,
                    new List<string>
                    {
                        ex.Message
                    });

            return StatusCode(
                respuesta.CodigoEstado,
                respuesta);
        }
    }
}