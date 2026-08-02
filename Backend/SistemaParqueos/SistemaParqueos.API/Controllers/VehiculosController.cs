using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.DTO.Vehiculo;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehiculosController : ControllerBase
{
    private readonly IVehiculoLN _vehiculoLN;

    public VehiculosController(
        IVehiculoLN vehiculoLN)
    {
        _vehiculoLN = vehiculoLN;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var respuesta =
            await _vehiculoLN.ObtenerTodosAsync();

        return CrearResultado(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var respuesta =
            await _vehiculoLN.ObtenerPorIdAsync(id);

        return CrearResultado(respuesta);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(
        [FromBody] VehiculoCrearDto vehiculoDto)
    {
        var respuesta =
            await _vehiculoLN.AgregarAsync(
                vehiculoDto);

        return CrearResultado(respuesta);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] VehiculoActualizarDto vehiculoDto)
    {
        var respuesta =
            await _vehiculoLN.ActualizarAsync(
                id,
                vehiculoDto);

        return CrearResultado(respuesta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var respuesta =
            await _vehiculoLN.EliminarAsync(id);

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