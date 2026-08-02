using SistemaParqueos.Dominio.DTO.Dashboard;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class DashboardLN : IDashboardLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;

    public DashboardLN(
        IUnidadTrabajoEF unidadTrabajo)
    {
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<Respuesta> ObtenerResumenAsync()
    {
        try
        {
            var fechaActualUtc = DateTime.UtcNow;

            var inicioDia =
                new DateTime(
                    fechaActualUtc.Year,
                    fechaActualUtc.Month,
                    fechaActualUtc.Day,
                    0,
                    0,
                    0,
                    DateTimeKind.Utc);

            var inicioDiaSiguiente =
                inicioDia.AddDays(1);

            var inicioMes =
                new DateTime(
                    fechaActualUtc.Year,
                    fechaActualUtc.Month,
                    1,
                    0,
                    0,
                    0,
                    DateTimeKind.Utc);

            var inicioMesSiguiente =
                inicioMes.AddMonths(1);

            var vehiculosDentro =
                await _unidadTrabajo.IngresosVehiculo
                    .ContarAsync(
                        ingreso =>
                            ingreso.FechaSalida == null &&
                            ingreso.Estado == "Ingresado");

            var espaciosDisponibles =
                await _unidadTrabajo.EspaciosParqueo
                    .ContarAsync(
                        espacio =>
                            espacio.Activo &&
                            espacio.Disponible);

            var espaciosOcupados =
                await _unidadTrabajo.EspaciosParqueo
                    .ContarAsync(
                        espacio =>
                            espacio.Activo &&
                            !espacio.Disponible);

            var totalEspaciosActivos =
                await _unidadTrabajo.EspaciosParqueo
                    .ContarAsync(
                        espacio => espacio.Activo);

            var ingresosHoy =
                await _unidadTrabajo.IngresosVehiculo
                    .ContarAsync(
                        ingreso =>
                            ingreso.FechaIngreso >= inicioDia &&
                            ingreso.FechaIngreso <
                                inicioDiaSiguiente);

            var clientesActivos =
                await _unidadTrabajo.Clientes
                    .ContarAsync(
                        cliente => cliente.Activo);

            var vehiculosActivos =
                await _unidadTrabajo.Vehiculos
                    .ContarAsync(
                        vehiculo => vehiculo.Activo);

            var facturasHoy =
                await _unidadTrabajo.Facturas
                    .BuscarAsync(
                        factura =>
                            factura.FechaFactura >= inicioDia &&
                            factura.FechaFactura <
                                inicioDiaSiguiente);

            var facturasMes =
                await _unidadTrabajo.Facturas
                    .BuscarAsync(
                        factura =>
                            factura.FechaFactura >= inicioMes &&
                            factura.FechaFactura <
                                inicioMesSiguiente);

            var facturacionHoy =
                facturasHoy.Sum(
                    factura => factura.MontoTotal);

            var facturacionMes =
                facturasMes.Sum(
                    factura => factura.MontoTotal);

            decimal porcentajeOcupacion = 0;

            if (totalEspaciosActivos > 0)
            {
                porcentajeOcupacion =
                    Math.Round(
                        (decimal)espaciosOcupados /
                        totalEspaciosActivos *
                        100,
                        2);
            }

            var ingresosRecientes =
                await _unidadTrabajo.IngresosVehiculo
                    .ListarAsync(
                        ingreso => ingreso.Vehiculo,
                        ingreso => ingreso.Vehiculo.Cliente,
                        ingreso => ingreso.Espacio,
                        ingreso => ingreso.Espacio.Parqueo);

            var actividadReciente =
                ingresosRecientes
                    .OrderByDescending(
                        ingreso => ingreso.FechaIngreso)
                    .Take(5)
                    .Select(
                        ingreso =>
                            new ActividadRecienteDto
                            {
                                IngresoId =
                                    ingreso.IngresoId,

                                Placa =
                                    ingreso.Vehiculo.Placa,

                                Cliente =
                                    $"{ingreso.Vehiculo.Cliente.Nombre} " +
                                    $"{ingreso.Vehiculo.Cliente.Apellidos}",

                                Parqueo =
                                    ingreso.Espacio
                                        .Parqueo.NombreParqueo,

                                NumeroEspacio =
                                    ingreso.Espacio
                                        .NumeroEspacio,

                                FechaIngreso =
                                    ingreso.FechaIngreso,

                                FechaSalida =
                                    ingreso.FechaSalida,

                                Estado =
                                    ingreso.Estado
                            })
                    .ToList();

            var resumen =
                new DashboardResumenDto
                {
                    VehiculosDentro =
                        vehiculosDentro,

                    EspaciosDisponibles =
                        espaciosDisponibles,

                    EspaciosOcupados =
                        espaciosOcupados,

                    TotalEspaciosActivos =
                        totalEspaciosActivos,

                    IngresosHoy =
                        ingresosHoy,

                    FacturacionHoy =
                        facturacionHoy,

                    FacturacionMes =
                        facturacionMes,

                    ClientesActivos =
                        clientesActivos,

                    VehiculosActivos =
                        vehiculosActivos,

                    PorcentajeOcupacion =
                        porcentajeOcupacion,

                    ActividadReciente =
                        actividadReciente
                };

            return Respuesta.Correcta(
                resumen,
                "Resumen del dashboard consultado correctamente.");
        }
        catch (Exception ex)
        {
            return Respuesta.Fallida(
                "Ocurrió un error al consultar el dashboard.",
                500,
                new List<string>
                {
                    ex.Message
                });
        }
    }
}