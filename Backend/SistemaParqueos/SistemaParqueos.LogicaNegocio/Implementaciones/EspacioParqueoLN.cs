using SistemaParqueos.Dominio.DTO.EspacioParqueo;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class EspacioParqueoLN : IEspacioParqueoLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;

    public EspacioParqueoLN(
        IUnidadTrabajoEF unidadTrabajo)
    {
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<Respuesta> ObtenerTodosAsync()
    {
        try
        {
            var espacios =
                await _unidadTrabajo.EspaciosParqueo
                    .ListarAsync(
                        espacio => espacio.Parqueo);

            var resultado = espacios
                .OrderBy(espacio =>
                    espacio.Parqueo.NombreParqueo)
                .ThenBy(espacio =>
                    espacio.NumeroEspacio)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Espacios de parqueo consultados correctamente.");
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
                    "El identificador del espacio debe ser mayor que cero.",
                    400);
            }

            var espacio =
                await _unidadTrabajo.EspaciosParqueo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.EspacioId == id,
                        registro => registro.Parqueo);

            if (espacio is null)
            {
                return Respuesta.Fallida(
                    "El espacio de parqueo solicitado no existe.",
                    404);
            }

            return Respuesta.Correcta(
                MapearRespuesta(espacio),
                "Espacio de parqueo consultado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ObtenerPorParqueoAsync(
        int parqueoId)
    {
        try
        {
            if (parqueoId <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del parqueo debe ser mayor que cero.",
                    400);
            }

            var parqueo =
                await _unidadTrabajo.Parqueos
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.ParqueoId == parqueoId);

            if (parqueo is null)
            {
                return Respuesta.Fallida(
                    "El parqueo solicitado no existe.",
                    404);
            }

            var espacios =
                await _unidadTrabajo.EspaciosParqueo
                    .BuscarAsync(
                        registro =>
                            registro.ParqueoId == parqueoId,
                        registro => registro.Parqueo);

            var resultado = espacios
                .OrderBy(espacio =>
                    espacio.NumeroEspacio)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Espacios del parqueo consultados correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> AgregarAsync(
        EspacioParqueoCrearDto espacioDto)
    {
        try
        {
            NormalizarDatos(espacioDto);

            var parqueo =
                await _unidadTrabajo.Parqueos
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.ParqueoId ==
                            espacioDto.ParqueoId);

            if (parqueo is null)
            {
                return Respuesta.Fallida(
                    "El parqueo seleccionado no existe.",
                    400);
            }

            if (!parqueo.Activo)
            {
                return Respuesta.Fallida(
                    "El parqueo seleccionado está inactivo.",
                    409);
            }

            var espacioDuplicado =
                await _unidadTrabajo.EspaciosParqueo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.ParqueoId ==
                            espacioDto.ParqueoId &&
                            registro.NumeroEspacio ==
                            espacioDto.NumeroEspacio);

            if (espacioDuplicado is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe un espacio con ese número en el parqueo seleccionado.",
                    409);
            }

            var cantidadEspacios =
                await _unidadTrabajo.EspaciosParqueo
                    .ContarAsync(
                        registro =>
                            registro.ParqueoId ==
                            espacioDto.ParqueoId &&
                            registro.Activo);

            if (cantidadEspacios >= parqueo.CapacidadTotal)
            {
                return Respuesta.Fallida(
                    "El parqueo ya alcanzó su capacidad máxima de espacios.",
                    409);
            }

            var espacio = new EspacioParqueo
            {
                ParqueoId = espacioDto.ParqueoId,
                NumeroEspacio = espacioDto.NumeroEspacio,
                Disponible = espacioDto.Disponible,
                Activo = true,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = "API"
            };

            await _unidadTrabajo.EspaciosParqueo
                .InsertarAsync(espacio);

            await _unidadTrabajo.CompletarAsync();

            var espacioCreado =
                await _unidadTrabajo.EspaciosParqueo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.EspacioId ==
                            espacio.EspacioId,
                        registro => registro.Parqueo);

            return Respuesta.Correcta(
                MapearRespuesta(espacioCreado!),
                "Espacio de parqueo registrado correctamente.",
                201);
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ActualizarAsync(
        int id,
        EspacioParqueoActualizarDto espacioDto)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del espacio debe ser mayor que cero.",
                    400);
            }

            NormalizarDatos(espacioDto);

            var espacioExistente =
                await _unidadTrabajo.EspaciosParqueo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.EspacioId == id);

            if (espacioExistente is null)
            {
                return Respuesta.Fallida(
                    "El espacio de parqueo solicitado no existe.",
                    404);
            }

            var parqueo =
                await _unidadTrabajo.Parqueos
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.ParqueoId ==
                            espacioDto.ParqueoId);

            if (parqueo is null)
            {
                return Respuesta.Fallida(
                    "El parqueo seleccionado no existe.",
                    400);
            }

            if (!parqueo.Activo)
            {
                return Respuesta.Fallida(
                    "El parqueo seleccionado está inactivo.",
                    409);
            }

            var duplicado =
                await _unidadTrabajo.EspaciosParqueo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.ParqueoId ==
                            espacioDto.ParqueoId &&
                            registro.NumeroEspacio ==
                            espacioDto.NumeroEspacio &&
                            registro.EspacioId != id);

            if (duplicado is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe otro espacio con ese número en el parqueo seleccionado.",
                    409);
            }

            if (espacioExistente.ParqueoId !=
                espacioDto.ParqueoId)
            {
                var cantidadEspaciosDestino =
                    await _unidadTrabajo.EspaciosParqueo
                        .ContarAsync(
                            registro =>
                                registro.ParqueoId ==
                                espacioDto.ParqueoId &&
                                registro.Activo);

                if (cantidadEspaciosDestino >=
                    parqueo.CapacidadTotal)
                {
                    return Respuesta.Fallida(
                        "El parqueo de destino ya alcanzó su capacidad máxima.",
                        409);
                }
            }

            espacioExistente.ParqueoId =
                espacioDto.ParqueoId;

            espacioExistente.NumeroEspacio =
                espacioDto.NumeroEspacio;

            espacioExistente.Disponible =
                espacioDto.Disponible;

            espacioExistente.Activo =
                espacioDto.Activo;

            espacioExistente.ActualizadoEn =
                DateTime.UtcNow;

            espacioExistente.ActualizadoPor =
                "API";

            _unidadTrabajo.EspaciosParqueo
                .Modificar(espacioExistente);

            await _unidadTrabajo.CompletarAsync();

            var espacioActualizado =
                await _unidadTrabajo.EspaciosParqueo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.EspacioId == id,
                        registro => registro.Parqueo);

            return Respuesta.Correcta(
                MapearRespuesta(espacioActualizado!),
                "Espacio de parqueo actualizado correctamente.");
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
                    "El identificador del espacio debe ser mayor que cero.",
                    400);
            }

            var espacio =
                await _unidadTrabajo.EspaciosParqueo
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.EspacioId == id);

            if (espacio is null)
            {
                return Respuesta.Fallida(
                    "El espacio de parqueo solicitado no existe.",
                    404);
            }

            var ingresoActivo =
                await _unidadTrabajo.IngresosVehiculo
                    .ObtenerEntidadAsync(
                        ingreso =>
                            ingreso.EspacioId == id &&
                            ingreso.FechaSalida == null);

            if (ingresoActivo is not null)
            {
                return Respuesta.Fallida(
                    "No se puede desactivar el espacio porque tiene un vehículo ingresado.",
                    409);
            }

            espacio.Activo = false;
            espacio.Disponible = false;
            espacio.ActualizadoEn = DateTime.UtcNow;
            espacio.ActualizadoPor = "API";

            _unidadTrabajo.EspaciosParqueo
                .Modificar(espacio);

            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                true,
                "Espacio de parqueo desactivado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    private static EspacioParqueoRespuestaDto MapearRespuesta(
        EspacioParqueo espacio)
    {
        return new EspacioParqueoRespuestaDto
        {
            EspacioId = espacio.EspacioId,
            ParqueoId = espacio.ParqueoId,
            NombreParqueo =
                espacio.Parqueo.NombreParqueo,
            NumeroEspacio =
                espacio.NumeroEspacio,
            Disponible =
                espacio.Disponible,
            Activo =
                espacio.Activo,
            CreadoEn =
                espacio.CreadoEn
        };
    }

    private static void NormalizarDatos(
        EspacioParqueoCrearDto espacioDto)
    {
        espacioDto.NumeroEspacio =
            espacioDto.NumeroEspacio
                .Trim()
                .ToUpperInvariant();
    }

    private static void NormalizarDatos(
        EspacioParqueoActualizarDto espacioDto)
    {
        espacioDto.NumeroEspacio =
            espacioDto.NumeroEspacio
                .Trim()
                .ToUpperInvariant();
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