using SistemaParqueos.Dominio.DTO.Tarifa;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface ITarifaLN
{
    Task<Respuesta> ObtenerTodosAsync();

    Task<Respuesta> ObtenerPorIdAsync(int id);

    Task<Respuesta> ObtenerPorTipoVehiculoAsync(
        int tipoVehiculoId);

    Task<Respuesta> AgregarAsync(
        TarifaCrearDto tarifaDto);

    Task<Respuesta> ActualizarAsync(
        int id,
        TarifaActualizarDto tarifaDto);

    Task<Respuesta> EliminarAsync(int id);
}