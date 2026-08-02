using Microsoft.EntityFrameworkCore.Storage;
using SistemaParqueos.AccesoDatos.Contexto;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;

namespace SistemaParqueos.AccesoDatos.Implementaciones;

public class UnidadTrabajoEF : IUnidadTrabajoEF
{
    private readonly ParqueosDbContext _contexto;
    private IDbContextTransaction? _transaccion;
    private IRepositorioAD<Rol>? _roles;
    private IRepositorioAD<Usuario>? _usuarios;
    private IRepositorioAD<Cliente>? _clientes;
    private IRepositorioAD<TipoVehiculo>? _tiposVehiculo;
    private IRepositorioAD<Vehiculo>? _vehiculos;
    private IRepositorioAD<Parqueo>? _parqueos;
    private IRepositorioAD<EspacioParqueo>? _espaciosParqueo;
    private IRepositorioAD<Tarifa>? _tarifas;
    private IRepositorioAD<IngresoVehiculo>? _ingresosVehiculo;
    private IRepositorioAD<Factura>? _facturas;

    public UnidadTrabajoEF(ParqueosDbContext contexto)
    {
        _contexto = contexto;
    }


    public IRepositorioAD<Rol> Roles =>
    _roles ??=
        new RepositorioAD<Rol>(_contexto);

    public IRepositorioAD<Usuario> Usuarios =>
        _usuarios ??=
            new RepositorioAD<Usuario>(_contexto);
    public IRepositorioAD<Cliente> Clientes =>
        _clientes ??= new RepositorioAD<Cliente>(_contexto);

    public IRepositorioAD<TipoVehiculo> TiposVehiculo =>
        _tiposVehiculo ??= new RepositorioAD<TipoVehiculo>(_contexto);

    public IRepositorioAD<Vehiculo> Vehiculos =>
        _vehiculos ??= new RepositorioAD<Vehiculo>(_contexto);

    public IRepositorioAD<Parqueo> Parqueos =>
        _parqueos ??= new RepositorioAD<Parqueo>(_contexto);

    public IRepositorioAD<EspacioParqueo> EspaciosParqueo =>
        _espaciosParqueo ??=
            new RepositorioAD<EspacioParqueo>(_contexto);

    public IRepositorioAD<Tarifa> Tarifas =>
        _tarifas ??= new RepositorioAD<Tarifa>(_contexto);

    public IRepositorioAD<IngresoVehiculo> IngresosVehiculo =>
        _ingresosVehiculo ??=
            new RepositorioAD<IngresoVehiculo>(_contexto);

    public IRepositorioAD<Factura> Facturas =>
        _facturas ??= new RepositorioAD<Factura>(_contexto);

    public async Task<int> CompletarAsync()
    {
        return await _contexto.SaveChangesAsync();
    }

    public async Task EmpezarTransaccionAsync()
    {
        if (_transaccion is not null)
        {
            throw new InvalidOperationException(
                "Ya existe una transacción activa.");
        }

        _transaccion =
            await _contexto.Database.BeginTransactionAsync();
    }

    public async Task CompletarTransaccionAsync()
    {
        if (_transaccion is null)
        {
            throw new InvalidOperationException(
                "No existe una transacción activa.");
        }

        try
        {
            await _contexto.SaveChangesAsync();
            await _transaccion.CommitAsync();
        }
        catch
        {
            await _transaccion.RollbackAsync();
            throw;
        }
        finally
        {
            await _transaccion.DisposeAsync();
            _transaccion = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaccion is null)
        {
            return;
        }

        await _transaccion.RollbackAsync();
        await _transaccion.DisposeAsync();

        _transaccion = null;
    }

    public void Dispose()
    {
        _transaccion?.Dispose();
        _contexto.Dispose();
    }
}