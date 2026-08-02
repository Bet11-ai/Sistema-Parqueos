using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.DTO.Cliente;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{
    private readonly IClienteLN _clienteLN;

    public ClientesController(IClienteLN clienteLN)
    {
        _clienteLN = clienteLN;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var respuesta =
            await _clienteLN.ObtenerTodosAsync();

        return CrearResultado(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var respuesta =
            await _clienteLN.ObtenerPorIdAsync(id);

        return CrearResultado(respuesta);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(
        [FromBody] ClienteCrearDto clienteDto)
    {
        var respuesta =
            await _clienteLN.AgregarAsync(clienteDto);

        return CrearResultado(respuesta);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ClienteActualizarDto clienteDto)
    {
        var respuesta =
            await _clienteLN.ActualizarAsync(
                id,
                clienteDto);

        return CrearResultado(respuesta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var respuesta =
            await _clienteLN.EliminarAsync(id);

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