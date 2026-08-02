namespace SistemaParqueos.Dominio.DTO.Dashboard;

public class DashboardResumenDto
{
    public int VehiculosDentro { get; set; }

    public int EspaciosDisponibles { get; set; }

    public int EspaciosOcupados { get; set; }

    public int TotalEspaciosActivos { get; set; }

    public int IngresosHoy { get; set; }

    public decimal FacturacionHoy { get; set; }

    public decimal FacturacionMes { get; set; }

    public int ClientesActivos { get; set; }

    public int VehiculosActivos { get; set; }

    public decimal PorcentajeOcupacion { get; set; }

    public List<ActividadRecienteDto> ActividadReciente { get; set; }
        = new();
}