using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaParqueos.Dominio.DTO.Auth;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthLN _authLN;

    public AuthController(IAuthLN authLN)
    {
        _authLN = authLN;
    }

    [AllowAnonymous]
    [HttpPost("primer-administrador")]
    public async Task<IActionResult>
        CrearPrimerAdministrador(
            [FromBody]
            CrearPrimerAdministradorDto dto)
    {
        var respuesta =
            await _authLN
                .CrearPrimerAdministradorAsync(dto);

        return CrearResultado(respuesta);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto)
    {
        var respuesta =
            await _authLN.IniciarSesionAsync(dto);

        return CrearResultado(respuesta);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("usuarios")]
    public async Task<IActionResult> RegistrarUsuario(
        [FromBody] RegistrarUsuarioDto dto)
    {
        var respuesta =
            await _authLN.RegistrarUsuarioAsync(dto);

        return CrearResultado(respuesta);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet("roles")]
    public async Task<IActionResult> ObtenerRoles()
    {
        var respuesta =
            await _authLN.ObtenerRolesAsync();

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