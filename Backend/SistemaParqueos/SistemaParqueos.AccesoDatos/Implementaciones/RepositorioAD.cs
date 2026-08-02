using Microsoft.EntityFrameworkCore;
using SistemaParqueos.Dominio.InterfacesAD;
using System.Linq.Expressions;

namespace SistemaParqueos.AccesoDatos.Implementaciones;

public class RepositorioAD<TEntity> : IRepositorioAD<TEntity>
    where TEntity : class
{
    protected readonly DbContext Contexto;
    protected readonly DbSet<TEntity> Entidades;

    public RepositorioAD(DbContext contexto)
    {
        Contexto = contexto;
        Entidades = contexto.Set<TEntity>();
    }

    public async Task<TEntity> InsertarAsync(TEntity entidad)
    {
        await Entidades.AddAsync(entidad);
        return entidad;
    }

    public void Modificar(TEntity entidad)
    {
        Entidades.Update(entidad);
    }

    public void Eliminar(TEntity entidad)
    {
        Entidades.Remove(entidad);
    }

    public async Task<List<TEntity>> ListarAsync(
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> consulta = Entidades.AsNoTracking();

        consulta = AplicarIncludes(consulta, includes);

        return await consulta.ToListAsync();
    }

    public async Task<List<TEntity>> BuscarAsync(
        Expression<Func<TEntity, bool>> predicado,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> consulta = Entidades
            .AsNoTracking()
            .Where(predicado);

        consulta = AplicarIncludes(consulta, includes);

        return await consulta.ToListAsync();
    }

    public async Task<TEntity?> ObtenerEntidadAsync(
        Expression<Func<TEntity, bool>> predicado,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> consulta = Entidades.AsNoTracking();

        consulta = AplicarIncludes(consulta, includes);

        return await consulta.FirstOrDefaultAsync(predicado);
    }

    public async Task<int> ContarAsync(
        Expression<Func<TEntity, bool>> predicado)
    {
        return await Entidades.CountAsync(predicado);
    }

    private static IQueryable<TEntity> AplicarIncludes(
        IQueryable<TEntity> consulta,
        IEnumerable<Expression<Func<TEntity, object>>> includes)
    {
        foreach (var include in includes)
        {
            consulta = consulta.Include(include);
        }

        return consulta;
    }
}