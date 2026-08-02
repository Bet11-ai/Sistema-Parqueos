using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface IDashboardLN
{
    Task<Respuesta> ObtenerResumenAsync();
}