namespace SistemaParqueos.Dominio.DTO.EspacioParqueo;

public class EspacioParqueoRespuestaDto
{
    public int EspacioId { get; set; }

    public int ParqueoId { get; set; }

    public string NombreParqueo { get; set; } = string.Empty;

    public string NumeroEspacio { get; set; } = string.Empty;

    public bool Disponible { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }
}