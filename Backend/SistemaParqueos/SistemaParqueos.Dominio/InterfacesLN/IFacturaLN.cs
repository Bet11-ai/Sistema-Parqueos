using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface IFacturaLN
{
    Task<Respuesta> ObtenerTodasAsync();

    Task<Respuesta> ObtenerPorIdAsync(int id);

    Task<Respuesta> ObtenerPorIngresoAsync(int ingresoId);
}