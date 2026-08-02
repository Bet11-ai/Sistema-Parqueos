using SistemaParqueos.Dominio.DTO.Tarifa;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class TarifaLN : ITarifaLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;

    public TarifaLN(IUnidadTrabajoEF unidadTrabajo)
    {
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<Respuesta> ObtenerTodosAsync()
    {
        try
        {
            var tarifas =
                await _unidadTrabajo.Tarifas.ListarAsync(
                    tarifa => tarifa.TipoVehiculo);

            var resultado = tarifas
                .OrderBy(tarifa =>
                    tarifa.TipoVehiculo.Descripcion)
                .ThenBy(tarifa => tarifa.Descripcion)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Tarifas consultadas correctamente.");
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
                    "El identificador de la tarifa debe ser mayor que cero.",
                    400);
            }

            var tarifa =
                await _unidadTrabajo.Tarifas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TarifaId == id,
                        registro => registro.TipoVehiculo);

            if (tarifa is null)
            {
                return Respuesta.Fallida(
                    "La tarifa solicitada no existe.",
                    404);
            }

            return Respuesta.Correcta(
                MapearRespuesta(tarifa),
                "Tarifa consultada correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ObtenerPorTipoVehiculoAsync(
        int tipoVehiculoId)
    {
        try
        {
            if (tipoVehiculoId <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del tipo de vehículo debe ser mayor que cero.",
                    400);
            }

            var tipoVehiculo =
                await _unidadTrabajo.TiposVehiculo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TipoVehiculoId ==
                            tipoVehiculoId);

            if (tipoVehiculo is null)
            {
                return Respuesta.Fallida(
                    "El tipo de vehículo solicitado no existe.",
                    404);
            }

            var tarifas =
                await _unidadTrabajo.Tarifas.BuscarAsync(
                    registro =>
                        registro.TipoVehiculoId ==
                        tipoVehiculoId,
                    registro => registro.TipoVehiculo);

            var resultado = tarifas
                .OrderBy(tarifa => tarifa.Descripcion)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Tarifas del tipo de vehículo consultadas correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> AgregarAsync(
        TarifaCrearDto tarifaDto)
    {
        try
        {
            NormalizarDatos(tarifaDto);

            var tipoVehiculo =
                await _unidadTrabajo.TiposVehiculo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TipoVehiculoId ==
                            tarifaDto.TipoVehiculoId);

            if (tipoVehiculo is null)
            {
                return Respuesta.Fallida(
                    "El tipo de vehículo seleccionado no existe.",
                    400);
            }

            if (!tipoVehiculo.Activo)
            {
                return Respuesta.Fallida(
                    "El tipo de vehículo seleccionado está inactivo.",
                    409);
            }

            if (tarifaDto.MontoHora <= 0)
            {
                return Respuesta.Fallida(
                    "El monto por hora debe ser mayor que cero.",
                    400);
            }

            var tarifaDuplicada =
                await _unidadTrabajo.Tarifas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TipoVehiculoId ==
                            tarifaDto.TipoVehiculoId &&
                            registro.Descripcion ==
                            tarifaDto.Descripcion &&
                            registro.Activo);

            if (tarifaDuplicada is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe una tarifa activa con esa descripción para el tipo de vehículo.",
                    409);
            }

            var tarifa = new Tarifa
            {
                TipoVehiculoId =
                    tarifaDto.TipoVehiculoId,
                Descripcion =
                    tarifaDto.Descripcion,
                MontoHora =
                    tarifaDto.MontoHora,
                Activo = true,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = "API"
            };

            await _unidadTrabajo.Tarifas
                .InsertarAsync(tarifa);

            await _unidadTrabajo.CompletarAsync();

            var tarifaCreada =
                await _unidadTrabajo.Tarifas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TarifaId ==
                            tarifa.TarifaId,
                        registro => registro.TipoVehiculo);

            return Respuesta.Correcta(
                MapearRespuesta(tarifaCreada!),
                "Tarifa registrada correctamente.",
                201);
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ActualizarAsync(
        int id,
        TarifaActualizarDto tarifaDto)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador de la tarifa debe ser mayor que cero.",
                    400);
            }

            NormalizarDatos(tarifaDto);

            var tarifaExistente =
                await _unidadTrabajo.Tarifas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TarifaId == id);

            if (tarifaExistente is null)
            {
                return Respuesta.Fallida(
                    "La tarifa solicitada no existe.",
                    404);
            }

            var tipoVehiculo =
                await _unidadTrabajo.TiposVehiculo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TipoVehiculoId ==
                            tarifaDto.TipoVehiculoId);

            if (tipoVehiculo is null)
            {
                return Respuesta.Fallida(
                    "El tipo de vehículo seleccionado no existe.",
                    400);
            }

            if (!tipoVehiculo.Activo)
            {
                return Respuesta.Fallida(
                    "El tipo de vehículo seleccionado está inactivo.",
                    409);
            }

            if (tarifaDto.MontoHora <= 0)
            {
                return Respuesta.Fallida(
                    "El monto por hora debe ser mayor que cero.",
                    400);
            }

            var duplicada =
                await _unidadTrabajo.Tarifas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TipoVehiculoId ==
                            tarifaDto.TipoVehiculoId &&
                            registro.Descripcion ==
                            tarifaDto.Descripcion &&
                            registro.TarifaId != id &&
                            registro.Activo);

            if (duplicada is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe otra tarifa activa con esa descripción.",
                    409);
            }

            tarifaExistente.TipoVehiculoId =
                tarifaDto.TipoVehiculoId;

            tarifaExistente.Descripcion =
                tarifaDto.Descripcion;

            tarifaExistente.MontoHora =
                tarifaDto.MontoHora;

            tarifaExistente.Activo =
                tarifaDto.Activo;

            tarifaExistente.ActualizadoEn =
                DateTime.UtcNow;

            tarifaExistente.ActualizadoPor =
                "API";

            _unidadTrabajo.Tarifas
                .Modificar(tarifaExistente);

            await _unidadTrabajo.CompletarAsync();

            var tarifaActualizada =
                await _unidadTrabajo.Tarifas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TarifaId == id,
                        registro => registro.TipoVehiculo);

            return Respuesta.Correcta(
                MapearRespuesta(tarifaActualizada!),
                "Tarifa actualizada correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> EliminarAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador de la tarifa debe ser mayor que cero.",
                    400);
            }

            var tarifa =
                await _unidadTrabajo.Tarifas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.TarifaId == id);

            if (tarifa is null)
            {
                return Respuesta.Fallida(
                    "La tarifa solicitada no existe.",
                    404);
            }

            tarifa.Activo = false;
            tarifa.ActualizadoEn = DateTime.UtcNow;
            tarifa.ActualizadoPor = "API";

            _unidadTrabajo.Tarifas.Modificar(tarifa);
            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                true,
                "Tarifa desactivada correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    private static TarifaRespuestaDto MapearRespuesta(
        Tarifa tarifa)
    {
        return new TarifaRespuestaDto
        {
            TarifaId = tarifa.TarifaId,
            TipoVehiculoId =
                tarifa.TipoVehiculoId,
            TipoVehiculo =
                tarifa.TipoVehiculo.Descripcion,
            Descripcion =
                tarifa.Descripcion,
            MontoHora =
                tarifa.MontoHora,
            Activo =
                tarifa.Activo,
            CreadoEn =
                tarifa.CreadoEn
        };
    }

    private static void NormalizarDatos(
        TarifaCrearDto tarifaDto)
    {
        tarifaDto.Descripcion =
            tarifaDto.Descripcion.Trim();
    }

    private static void NormalizarDatos(
        TarifaActualizarDto tarifaDto)
    {
        tarifaDto.Descripcion =
            tarifaDto.Descripcion.Trim();
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