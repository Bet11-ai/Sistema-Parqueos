using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.DTO.TipoVehiculo;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TiposVehiculoController : ControllerBase
{
    private readonly ITipoVehiculoLN _tipoVehiculoLN;

    public TiposVehiculoController(
        ITipoVehiculoLN tipoVehiculoLN)
    {
        _tipoVehiculoLN = tipoVehiculoLN;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var respuesta =
            await _tipoVehiculoLN.ObtenerTodosAsync();

        return CrearResultado(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(
        int id)
    {
        var respuesta =
            await _tipoVehiculoLN.ObtenerPorIdAsync(id);

        return CrearResultado(respuesta);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(
        [FromBody]
        TipoVehiculoCrearDto dto)
    {
        var respuesta =
            await _tipoVehiculoLN.AgregarAsync(dto);

        return CrearResultado(respuesta);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody]
        TipoVehiculoActualizarDto dto)
    {
        var respuesta =
            await _tipoVehiculoLN.ActualizarAsync(
                id,
                dto);

        return CrearResultado(respuesta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(
        int id)
    {
        var respuesta =
            await _tipoVehiculoLN.EliminarAsync(id);

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