using SistemaParqueos.Dominio.DTO.IngresoVehiculo;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class IngresoVehiculoLN : IIngresoVehiculoLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;

    public IngresoVehiculoLN(
        IUnidadTrabajoEF unidadTrabajo)
    {
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<Respuesta> ObtenerTodosAsync()
    {
        try
        {
            var ingresos =
                await _unidadTrabajo.IngresosVehiculo
                    .ListarAsync(
                        ingreso => ingreso.Vehiculo,
                        ingreso => ingreso.Espacio,
                        ingreso => ingreso.Espacio.Parqueo);

            var resultado = ingresos
                .OrderByDescending(ingreso =>
                    ingreso.FechaIngreso)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Ingresos consultados correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ObtenerPorIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del ingreso debe ser mayor que cero.",
                    400);
            }

            var ingreso =
                await _unidadTrabajo.IngresosVehiculo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.IngresoId == id,
                        registro => registro.Vehiculo,
                        registro => registro.Espacio,
                        registro => registro.Espacio.Parqueo);

            if (ingreso is null)
            {
                return Respuesta.Fallida(
                    "El ingreso solicitado no existe.",
                    404);
            }

            return Respuesta.Correcta(
                MapearRespuesta(ingreso),
                "Ingreso consultado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ObtenerActivosAsync()
    {
        try
        {
            var ingresos =
                await _unidadTrabajo.IngresosVehiculo
                    .BuscarAsync(
                        ingreso =>
                            ingreso.FechaSalida == null &&
                            ingreso.Estado == "Ingresado",
                        ingreso => ingreso.Vehiculo,
                        ingreso => ingreso.Espacio,
                        ingreso => ingreso.Espacio.Parqueo);

            var resultado = ingresos
                .OrderByDescending(ingreso =>
                    ingreso.FechaIngreso)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Ingresos activos consultados correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> AgregarAsync(
        IngresoVehiculoCrearDto ingresoDto)
    {
        try
        {
            var vehiculo =
                await _unidadTrabajo.Vehiculos
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.VehiculoId ==
                            ingresoDto.VehiculoId);

            if (vehiculo is null)
            {
                return Respuesta.Fallida(
                    "El vehículo seleccionado no existe.",
                    400);
            }

            if (!vehiculo.Activo)
            {
                return Respuesta.Fallida(
                    "El vehículo seleccionado está inactivo.",
                    409);
            }

            var ingresoActivoVehiculo =
                await _unidadTrabajo.IngresosVehiculo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.VehiculoId ==
                            ingresoDto.VehiculoId &&
                            registro.FechaSalida == null);

            if (ingresoActivoVehiculo is not null)
            {
                return Respuesta.Fallida(
                    "El vehículo ya tiene un ingreso activo.",
                    409);
            }

            var espacio =
                await _unidadTrabajo.EspaciosParqueo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.EspacioId ==
                            ingresoDto.EspacioId,
                        registro => registro.Parqueo);

            if (espacio is null)
            {
                return Respuesta.Fallida(
                    "El espacio seleccionado no existe.",
                    400);
            }

            if (!espacio.Activo)
            {
                return Respuesta.Fallida(
                    "El espacio seleccionado está inactivo.",
                    409);
            }

            if (!espacio.Disponible)
            {
                return Respuesta.Fallida(
                    "El espacio seleccionado no está disponible.",
                    409);
            }

            if (!espacio.Parqueo.Activo)
            {
                return Respuesta.Fallida(
                    "El parqueo asociado al espacio está inactivo.",
                    409);
            }

            await _unidadTrabajo.EmpezarTransaccionAsync();

            var ingreso = new IngresoVehiculo
            {
                VehiculoId = ingresoDto.VehiculoId,
                EspacioId = ingresoDto.EspacioId,
                FechaIngreso = DateTime.UtcNow,
                FechaSalida = null,
                Estado = "Ingresado",
                CreadoEn = DateTime.UtcNow,
                CreadoPor = "API"
            };

            await _unidadTrabajo.IngresosVehiculo
                .InsertarAsync(ingreso);

            espacio.Disponible = false;
            espacio.ActualizadoEn = DateTime.UtcNow;
            espacio.ActualizadoPor = "API";

            _unidadTrabajo.EspaciosParqueo
                .Modificar(espacio);

            await _unidadTrabajo.CompletarTransaccionAsync();

            var ingresoCreado =
                await _unidadTrabajo.IngresosVehiculo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.IngresoId ==
                            ingreso.IngresoId,
                        registro => registro.Vehiculo,
                        registro => registro.Espacio,
                        registro => registro.Espacio.Parqueo);

            return Respuesta.Correcta(
                MapearRespuesta(ingresoCreado!),
                "Ingreso de vehículo registrado correctamente.",
                201);
        }
        catch (Exception ex)
        {
            await _unidadTrabajo.RollbackAsync();

            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> RegistrarSalidaAsync(
        int ingresoId)
    {
        try
        {
            if (ingresoId <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del ingreso debe ser mayor que cero.",
                    400);
            }

            var ingreso =
                await _unidadTrabajo.IngresosVehiculo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.IngresoId == ingresoId,
                        registro => registro.Vehiculo,
                        registro => registro.Vehiculo.TipoVehiculo,
                        registro => registro.Espacio,
                        registro => registro.Espacio.Parqueo);

            if (ingreso is null)
            {
                return Respuesta.Fallida(
                    "El ingreso solicitado no existe.",
                    404);
            }

            if (ingreso.FechaSalida is not null ||
                ingreso.Estado == "Finalizado")
            {
                return Respuesta.Fallida(
                    "Este ingreso ya tiene una salida registrada.",
                    409);
            }

            var facturaExistente =
                await _unidadTrabajo.Facturas
                    .ObtenerEntidadAsync(
                        factura =>
                            factura.IngresoId == ingresoId);

            if (facturaExistente is not null)
            {
                return Respuesta.Fallida(
                    "Este ingreso ya tiene una factura generada.",
                    409);
            }

            var tarifa =
                await _unidadTrabajo.Tarifas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TipoVehiculoId ==
                            ingreso.Vehiculo.TipoVehiculoId &&
                            registro.Activo);

            if (tarifa is null)
            {
                return Respuesta.Fallida(
                    "No existe una tarifa activa para el tipo de vehículo.",
                    409);
            }

            var fechaSalida = DateTime.UtcNow;

            var tiempoTranscurrido =
                fechaSalida - ingreso.FechaIngreso;

            var horasCobradas =
                (decimal)Math.Ceiling(
                    tiempoTranscurrido.TotalHours);

            if (horasCobradas < 1)
            {
                horasCobradas = 1;
            }

            var montoTotal =
                horasCobradas * tarifa.MontoHora;

            await _unidadTrabajo.EmpezarTransaccionAsync();

            ingreso.FechaSalida = fechaSalida;
            ingreso.Estado = "Finalizado";
            ingreso.ActualizadoEn = DateTime.UtcNow;
            ingreso.ActualizadoPor = "API";

            _unidadTrabajo.IngresosVehiculo
                .Modificar(ingreso);

            ingreso.Espacio.Disponible = true;
            ingreso.Espacio.ActualizadoEn = DateTime.UtcNow;
            ingreso.Espacio.ActualizadoPor = "API";

            _unidadTrabajo.EspaciosParqueo
                .Modificar(ingreso.Espacio);

            var factura = new Factura
            {
                IngresoId = ingreso.IngresoId,
                FechaFactura = fechaSalida,
                HorasCobradas = horasCobradas,
                MontoTotal = montoTotal,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = "API"
            };

            await _unidadTrabajo.Facturas
                .InsertarAsync(factura);

            await _unidadTrabajo.CompletarTransaccionAsync();

            var resultado =
                new SalidaVehiculoRespuestaDto
                {
                    IngresoId = ingreso.IngresoId,
                    Placa = ingreso.Vehiculo.Placa,
                    NumeroEspacio =
                        ingreso.Espacio.NumeroEspacio,
                    FechaIngreso =
                        ingreso.FechaIngreso,
                    FechaSalida =
                        fechaSalida,
                    HorasCobradas =
                        horasCobradas,
                    MontoHora =
                        tarifa.MontoHora,
                    MontoTotal =
                        montoTotal,
                    FacturaId =
                        factura.FacturaId,
                    Estado =
                        ingreso.Estado
                };

            return Respuesta.Correcta(
                resultado,
                "Salida registrada y factura generada correctamente.");
        }
        catch (Exception ex)
        {
            await _unidadTrabajo.RollbackAsync();

            return CrearRespuestaError(ex);
        }
    }

    private static IngresoVehiculoRespuestaDto MapearRespuesta(
        IngresoVehiculo ingreso)
    {
        return new IngresoVehiculoRespuestaDto
        {
            IngresoId = ingreso.IngresoId,
            VehiculoId = ingreso.VehiculoId,
            Placa = ingreso.Vehiculo.Placa,
            EspacioId = ingreso.EspacioId,
            NumeroEspacio =
                ingreso.Espacio.NumeroEspacio,
            NombreParqueo =
                ingreso.Espacio.Parqueo.NombreParqueo,
            FechaIngreso =
                ingreso.FechaIngreso,
            FechaSalida =
                ingreso.FechaSalida,
            Estado =
                ingreso.Estado
        };
    }

    private static Respuesta CrearRespuestaError(
        Exception ex)
    {
        return Respuesta.Fallida(
            "Ocurrió un error interno al procesar la solicitud.",
            500,
            new List<string>
            {
                ex.Message
            });
    }
}