using System.Linq.Expressions;

namespace SistemaParqueos.Dominio.InterfacesAD;

public interface IRepositorioAD<TEntity>
    where TEntity : class
{
    Task<TEntity> InsertarAsync(TEntity entidad);

    void Modificar(TEntity entidad);

    void Eliminar(TEntity entidad);

    Task<List<TEntity>> ListarAsync(
        params Expression<Func<TEntity, object>>[] includes);

    Task<List<TEntity>> BuscarAsync(
        Expression<Func<TEntity, bool>> predicado,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> ObtenerEntidadAsync(
        Expression<Func<TEntity, bool>> predicado,
        params Expression<Func<TEntity, object>>[] includes);

    Task<int> ContarAsync(
        Expression<Func<TEntity, bool>> predicado);
}