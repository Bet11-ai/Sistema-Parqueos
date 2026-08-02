using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.DTO.EspacioParqueo;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EspaciosParqueoController : ControllerBase
{
    private readonly IEspacioParqueoLN _espacioParqueoLN;

    public EspaciosParqueoController(
        IEspacioParqueoLN espacioParqueoLN)
    {
        _espacioParqueoLN = espacioParqueoLN;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var respuesta =
            await _espacioParqueoLN.ObtenerTodosAsync();

        return CrearResultado(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var respuesta =
            await _espacioParqueoLN.ObtenerPorIdAsync(id);

        return CrearResultado(respuesta);
    }

    [HttpGet("por-parqueo/{parqueoId:int}")]
    public async Task<IActionResult> ObtenerPorParqueo(
        int parqueoId)
    {
        var respuesta =
            await _espacioParqueoLN
                .ObtenerPorParqueoAsync(parqueoId);

        return CrearResultado(respuesta);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(
        [FromBody] EspacioParqueoCrearDto espacioDto)
    {
        var respuesta =
            await _espacioParqueoLN
                .AgregarAsync(espacioDto);

        return CrearResultado(respuesta);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody]
        EspacioParqueoActualizarDto espacioDto)
    {
        var respuesta =
            await _espacioParqueoLN
                .ActualizarAsync(id, espacioDto);

        return CrearResultado(respuesta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var respuesta =
            await _espacioParqueoLN.EliminarAsync(id);

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