using SistemaParqueos.Dominio.DTO.Parqueo;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface IParqueoLN
{
    Task<Respuesta> ObtenerTodosAsync();

    Task<Respuesta> ObtenerPorIdAsync(int id);

    Task<Respuesta> AgregarAsync(
        ParqueoCrearDto parqueoDto);

    Task<Respuesta> ActualizarAsync(
        int id,
        ParqueoActualizarDto parqueoDto);

    Task<Respuesta> EliminarAsync(int id);
}