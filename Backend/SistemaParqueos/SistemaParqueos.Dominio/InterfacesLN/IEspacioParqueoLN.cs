using SistemaParqueos.Dominio.DTO.EspacioParqueo;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface IEspacioParqueoLN
{
    Task<Respuesta> ObtenerTodosAsync();

    Task<Respuesta> ObtenerPorIdAsync(int id);

    Task<Respuesta> ObtenerPorParqueoAsync(int parqueoId);

    Task<Respuesta> AgregarAsync(
        EspacioParqueoCrearDto espacioDto);

    Task<Respuesta> ActualizarAsync(
        int id,
        EspacioParqueoActualizarDto espacioDto);

    Task<Respuesta> EliminarAsync(int id);
}