namespace SistemaParqueos.Dominio.DTO.Parqueo;

public class ParqueoRespuestaDto
{
    public int ParqueoId { get; set; }

    public string NombreParqueo { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public int CapacidadTotal { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }
}