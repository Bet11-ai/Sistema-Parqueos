using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.DTO.Parqueo;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParqueosController : ControllerBase
{
    private readonly IParqueoLN _parqueoLN;

    public ParqueosController(
        IParqueoLN parqueoLN)
    {
        _parqueoLN = parqueoLN;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var respuesta =
            await _parqueoLN.ObtenerTodosAsync();

        return CrearResultado(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var respuesta =
            await _parqueoLN.ObtenerPorIdAsync(id);

        return CrearResultado(respuesta);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(
        [FromBody] ParqueoCrearDto parqueoDto)
    {
        var respuesta =
            await _parqueoLN.AgregarAsync(parqueoDto);

        return CrearResultado(respuesta);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ParqueoActualizarDto parqueoDto)
    {
        var respuesta =
            await _parqueoLN.ActualizarAsync(
                id,
                parqueoDto);

        return CrearResultado(respuesta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var respuesta =
            await _parqueoLN.EliminarAsync(id);

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