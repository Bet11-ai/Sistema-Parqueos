namespace SistemaParqueos.Dominio.DTO.Dashboard;

public class ActividadRecienteDto
{
    public int IngresoId { get; set; }

    public string Placa { get; set; } = string.Empty;

    public string Cliente { get; set; } = string.Empty;

    public string Parqueo { get; set; } = string.Empty;

    public string NumeroEspacio { get; set; } = string.Empty;

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }

    public string Estado { get; set; } = string.Empty;
}