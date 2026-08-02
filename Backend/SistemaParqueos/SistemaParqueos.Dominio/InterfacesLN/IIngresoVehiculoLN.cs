using SistemaParqueos.Dominio.DTO.IngresoVehiculo;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface IIngresoVehiculoLN
{
    Task<Respuesta> ObtenerTodosAsync();

    Task<Respuesta> ObtenerPorIdAsync(int id);

    Task<Respuesta> ObtenerActivosAsync();

    Task<Respuesta> AgregarAsync(
        IngresoVehiculoCrearDto ingresoDto);

    Task<Respuesta> RegistrarSalidaAsync(int ingresoId);
}