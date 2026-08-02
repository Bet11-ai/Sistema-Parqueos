using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.DTO.IngresoVehiculo;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngresosVehiculoController : ControllerBase
{
    private readonly IIngresoVehiculoLN _ingresoVehiculoLN;

    public IngresosVehiculoController(
        IIngresoVehiculoLN ingresoVehiculoLN)
    {
        _ingresoVehiculoLN = ingresoVehiculoLN;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var respuesta =
            await _ingresoVehiculoLN.ObtenerTodosAsync();

        return CrearResultado(respuesta);
    }

    [HttpGet("activos")]
    public async Task<IActionResult> ObtenerActivos()
    {
        var respuesta =
            await _ingresoVehiculoLN.ObtenerActivosAsync();

        return CrearResultado(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var respuesta =
            await _ingresoVehiculoLN.ObtenerPorIdAsync(id);

        return CrearResultado(respuesta);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(
        [FromBody] IngresoVehiculoCrearDto ingresoDto)
    {
        var respuesta =
            await _ingresoVehiculoLN.AgregarAsync(ingresoDto);

        return CrearResultado(respuesta);
    }


    [HttpPut("{ingresoId:int}/salida")]
    public async Task<IActionResult> RegistrarSalida(int ingresoId)
    {
        var respuesta =
            await _ingresoVehiculoLN.RegistrarSalidaAsync(ingresoId);

        return CrearResultado(respuesta);
    }

    private IActionResult CrearResultado(
        Respuesta respuesta)
    {
        return StatusCode(
            respuesta.CodigoEstado,
            respuesta);
    }
}