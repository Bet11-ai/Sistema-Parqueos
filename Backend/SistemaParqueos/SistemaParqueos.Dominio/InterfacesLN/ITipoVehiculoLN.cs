using SistemaParqueos.Dominio.DTO.TipoVehiculo;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface ITipoVehiculoLN
{
    Task<Respuesta> ObtenerTodosAsync();

    Task<Respuesta> ObtenerPorIdAsync(int id);

    Task<Respuesta> AgregarAsync(TipoVehiculoCrearDto dto);

    Task<Respuesta> ActualizarAsync(int id, TipoVehiculoActualizarDto dto);

    Task<Respuesta> EliminarAsync(int id);
}