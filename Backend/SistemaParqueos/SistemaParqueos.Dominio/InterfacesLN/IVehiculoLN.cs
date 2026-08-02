using SistemaParqueos.Dominio.DTO.Vehiculo;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface IVehiculoLN
{
    Task<Respuesta> ObtenerTodosAsync();

    Task<Respuesta> ObtenerPorIdAsync(int id);

    Task<Respuesta> AgregarAsync(
        VehiculoCrearDto vehiculoDto);

    Task<Respuesta> ActualizarAsync(
        int id,
        VehiculoActualizarDto vehiculoDto);

    Task<Respuesta> EliminarAsync(int id);
}