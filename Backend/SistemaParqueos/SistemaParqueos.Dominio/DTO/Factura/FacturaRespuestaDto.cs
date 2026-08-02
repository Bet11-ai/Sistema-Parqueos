namespace SistemaParqueos.Dominio.DTO.Factura;

public class FacturaRespuestaDto
{
    public int FacturaId { get; set; }

    public int IngresoId { get; set; }

    public string Placa { get; set; } = string.Empty;

    public string TipoVehiculo { get; set; } = string.Empty;

    public string NumeroEspacio { get; set; } = string.Empty;

    public string NombreParqueo { get; set; } = string.Empty;

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }

    public DateTime FechaFactura { get; set; }

    public decimal HorasCobradas { get; set; }

    public decimal MontoTotal { get; set; }
}