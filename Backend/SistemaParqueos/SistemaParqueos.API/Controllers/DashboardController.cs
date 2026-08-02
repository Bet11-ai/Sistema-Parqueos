using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardLN _dashboardLN;

    public DashboardController(
        IDashboardLN dashboardLN)
    {
        _dashboardLN = dashboardLN;
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> ObtenerResumen()
    {
        var respuesta =
            await _dashboardLN.ObtenerResumenAsync();

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