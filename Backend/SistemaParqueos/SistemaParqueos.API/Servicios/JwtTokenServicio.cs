using Microsoft.IdentityModel.Tokens;

using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesLN;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaParqueos.API.Servicios;

public class JwtTokenServicio : ITokenServicio
{
    private readonly IConfiguration _configuracion;

    public JwtTokenServicio(
        IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    public string GenerarToken(
        Usuario usuario,
        DateTime fechaExpiracion)
    {
        var clave =
            _configuracion["Jwt:Clave"];

        var emisor =
            _configuracion["Jwt:Emisor"];

        var audiencia =
            _configuracion["Jwt:Audiencia"];

        if (string.IsNullOrWhiteSpace(clave) ||
            string.IsNullOrWhiteSpace(emisor) ||
            string.IsNullOrWhiteSpace(audiencia))
        {
            throw new InvalidOperationException(
                "La configuración JWT está incompleta.");
        }

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                usuario.UsuarioId.ToString()),

            new(
                ClaimTypes.Name,
                usuario.NombreCompleto),

            new(
                ClaimTypes.Email,
                usuario.Correo),

            new(
                ClaimTypes.Role,
                usuario.Rol.Nombre),

            new(
                "rolId",
                usuario.RolId.ToString())
        };

        var claveSeguridad =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(clave));

        var credenciales =
            new SigningCredentials(
                claveSeguridad,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: emisor,
            audience: audiencia,
            claims: claims,
            expires: fechaExpiracion,
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}