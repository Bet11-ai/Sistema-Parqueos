namespace SistemaParqueos.Dominio.DTO.TipoVehiculo;

public class TipoVehiculoRespuestaDto
{
    public int TipoVehiculoId { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }
}