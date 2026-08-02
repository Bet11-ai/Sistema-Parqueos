using SistemaParqueos.Dominio.DTO.Cliente;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface IClienteLN
{
    Task<Respuesta> ObtenerTodosAsync();

    Task<Respuesta> ObtenerPorIdAsync(int id);

    Task<Respuesta> AgregarAsync(
        ClienteCrearDto clienteDto);

    Task<Respuesta> ActualizarAsync(
        int id,
        ClienteActualizarDto clienteDto);

    Task<Respuesta> EliminarAsync(int id);
}