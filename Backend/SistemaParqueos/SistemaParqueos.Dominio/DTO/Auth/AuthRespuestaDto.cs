namespace SistemaParqueos.Dominio.DTO.Auth;

public class AuthRespuestaDto
{
    public int UsuarioId { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public int RolId { get; set; }

    public string Rol { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEn { get; set; }
}