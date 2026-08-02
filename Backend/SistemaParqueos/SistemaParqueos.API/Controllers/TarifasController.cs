using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.DTO.Tarifa;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TarifasController : ControllerBase
{
    private readonly ITarifaLN _tarifaLN;

    public TarifasController(ITarifaLN tarifaLN)
    {
        _tarifaLN = tarifaLN;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var respuesta =
            await _tarifaLN.ObtenerTodosAsync();

        return CrearResultado(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var respuesta =
            await _tarifaLN.ObtenerPorIdAsync(id);

        return CrearResultado(respuesta);
    }

    [HttpGet("por-tipo-vehiculo/{tipoVehiculoId:int}")]
    public async Task<IActionResult> ObtenerPorTipoVehiculo(
        int tipoVehiculoId)
    {
        var respuesta =
            await _tarifaLN
                .ObtenerPorTipoVehiculoAsync(
                    tipoVehiculoId);

        return CrearResultado(respuesta);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(
        [FromBody] TarifaCrearDto tarifaDto)
    {
        var respuesta =
            await _tarifaLN.AgregarAsync(tarifaDto);

        return CrearResultado(respuesta);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] TarifaActualizarDto tarifaDto)
    {
        var respuesta =
            await _tarifaLN.ActualizarAsync(
                id,
                tarifaDto);

        return CrearResultado(respuesta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var respuesta =
            await _tarifaLN.EliminarAsync(id);

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