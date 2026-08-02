namespace SistemaParqueos.Dominio.DTO.IngresoVehiculo;

public class IngresoVehiculoRespuestaDto
{
    public int IngresoId { get; set; }

    public int VehiculoId { get; set; }

    public string Placa { get; set; } = string.Empty;

    public int EspacioId { get; set; }

    public string NumeroEspacio { get; set; } = string.Empty;

    public string NombreParqueo { get; set; } = string.Empty;

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }

    public string Estado { get; set; } = string.Empty;
}