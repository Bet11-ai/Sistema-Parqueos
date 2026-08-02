namespace SistemaParqueos.Dominio.DTO.Vehiculo;

public class VehiculoRespuestaDto
{
    public int VehiculoId { get; set; }

    public int ClienteId { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public int TipoVehiculoId { get; set; }

    public string TipoVehiculo { get; set; } = string.Empty;

    public string Placa { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string? Modelo { get; set; }

    public string? Color { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }
}