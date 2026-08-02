using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FacturasController : ControllerBase
{
    private readonly IFacturaLN _facturaLN;

    public FacturasController(
        IFacturaLN facturaLN)
    {
        _facturaLN = facturaLN;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var respuesta =
            await _facturaLN.ObtenerTodasAsync();

        return CrearResultado(respuesta);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var respuesta =
            await _facturaLN.ObtenerPorIdAsync(id);

        return CrearResultado(respuesta);
    }

    [HttpGet("por-ingreso/{ingresoId:int}")]
    public async Task<IActionResult> ObtenerPorIngreso(
        int ingresoId)
    {
        var respuesta =
            await _facturaLN
                .ObtenerPorIngresoAsync(ingresoId);

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