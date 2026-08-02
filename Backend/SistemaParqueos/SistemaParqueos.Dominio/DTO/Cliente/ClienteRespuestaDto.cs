namespace SistemaParqueos.Dominio.DTO.Cliente;

public class ClienteRespuestaDto
{
    public int ClienteId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Cedula { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }
}