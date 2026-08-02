namespace SistemaParqueos.Dominio.DTO.IngresoVehiculo;

public class SalidaVehiculoRespuestaDto
{
    public int IngresoId { get; set; }

    public string Placa { get; set; } = string.Empty;

    public string NumeroEspacio { get; set; } = string.Empty;

    public DateTime FechaIngreso { get; set; }

    public DateTime FechaSalida { get; set; }

    public decimal HorasCobradas { get; set; }

    public decimal MontoHora { get; set; }

    public decimal MontoTotal { get; set; }

    public int FacturaId { get; set; }

    public string Estado { get; set; } = string.Empty;
}