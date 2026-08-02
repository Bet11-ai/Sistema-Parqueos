using SistemaParqueos.Dominio.Entidades;

namespace SistemaParqueos.Dominio.InterfacesAD;

public interface IUnidadTrabajoEF : IDisposable
{

    IRepositorioAD<Rol> Roles { get; }

    IRepositorioAD<Usuario> Usuarios { get; }
    IRepositorioAD<Cliente> Clientes { get; }

    IRepositorioAD<TipoVehiculo> TiposVehiculo { get; }

    IRepositorioAD<Vehiculo> Vehiculos { get; }

    IRepositorioAD<Parqueo> Parqueos { get; }

    IRepositorioAD<EspacioParqueo> EspaciosParqueo { get; }

    IRepositorioAD<Tarifa> Tarifas { get; }

    IRepositorioAD<IngresoVehiculo> IngresosVehiculo { get; }

    IRepositorioAD<Factura> Facturas { get; }

    Task<int> CompletarAsync();

    Task EmpezarTransaccionAsync();

    Task CompletarTransaccionAsync();

    Task RollbackAsync();
}