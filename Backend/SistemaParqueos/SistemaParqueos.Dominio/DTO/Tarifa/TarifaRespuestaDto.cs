namespace SistemaParqueos.Dominio.DTO.Tarifa;

public class TarifaRespuestaDto
{
    public int TarifaId { get; set; }

    public int TipoVehiculoId { get; set; }

    public string TipoVehiculo { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public decimal MontoHora { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }
}