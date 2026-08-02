using SistemaParqueos.Dominio.DTO.TipoVehiculo;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class TipoVehiculoLN : ITipoVehiculoLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;

    public TipoVehiculoLN(
        IUnidadTrabajoEF unidadTrabajo)
    {
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<Respuesta> ObtenerTodosAsync()
    {
        try
        {
            var tiposVehiculo =
                await _unidadTrabajo.TiposVehiculo
                    .BuscarAsync(tipo => true);

            var resultado = tiposVehiculo
                .OrderBy(tipo => tipo.Descripcion)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Tipos de vehículo consultados correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ObtenerPorIdAsync(
        int id)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del tipo de vehículo debe ser mayor que cero.",
                    400);
            }

            var tipoVehiculo =
                await _unidadTrabajo.TiposVehiculo
                    .ObtenerEntidadAsync(
                        tipo =>
                            tipo.TipoVehiculoId == id);

            if (tipoVehiculo is null)
            {
                return Respuesta.Fallida(
                    "El tipo de vehículo no existe.",
                    404);
            }

            return Respuesta.Correcta(
                MapearRespuesta(tipoVehiculo),
                "Tipo de vehículo consultado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> AgregarAsync(
        TipoVehiculoCrearDto dto)
    {
        try
        {
            dto.Descripcion =
                dto.Descripcion.Trim();

            if (string.IsNullOrWhiteSpace(
                dto.Descripcion))
            {
                return Respuesta.Fallida(
                    "La descripción es obligatoria.",
                    400);
            }

            var descripcionNormalizada =
                dto.Descripcion.ToUpperInvariant();

            var tipoExistente =
                await _unidadTrabajo.TiposVehiculo
                    .ObtenerEntidadAsync(
                        tipo =>
                            tipo.Descripcion
                                .ToUpper() ==
                            descripcionNormalizada);

            if (tipoExistente is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe un tipo de vehículo con esa descripción.",
                    409);
            }

            var tipoVehiculo =
                new TipoVehiculo
                {
                    Descripcion =
                        dto.Descripcion,

                    Activo = true,

                    CreadoEn =
                        DateTime.UtcNow,

                    CreadoPor =
                        "SISTEMA"
                };

            await _unidadTrabajo.TiposVehiculo
                .InsertarAsync(tipoVehiculo);

            await _unidadTrabajo
                .CompletarAsync();

            return Respuesta.Correcta(
                MapearRespuesta(tipoVehiculo),
                "Tipo de vehículo registrado correctamente.",
                201);
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ActualizarAsync(
        int id,
        TipoVehiculoActualizarDto dto)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del tipo de vehículo debe ser mayor que cero.",
                    400);
            }

            dto.Descripcion =
                dto.Descripcion.Trim();

            if (string.IsNullOrWhiteSpace(
                dto.Descripcion))
            {
                return Respuesta.Fallida(
                    "La descripción es obligatoria.",
                    400);
            }

            var tipoVehiculo =
                await _unidadTrabajo.TiposVehiculo
                    .ObtenerEntidadAsync(
                        tipo =>
                            tipo.TipoVehiculoId == id);

            if (tipoVehiculo is null)
            {
                return Respuesta.Fallida(
                    "El tipo de vehículo no existe.",
                    404);
            }

            var descripcionNormalizada =
                dto.Descripcion.ToUpperInvariant();

            var tipoDuplicado =
                await _unidadTrabajo.TiposVehiculo
                    .ObtenerEntidadAsync(
                        tipo =>
                            tipo.TipoVehiculoId != id &&
                            tipo.Descripcion
                                .ToUpper() ==
                            descripcionNormalizada);

            if (tipoDuplicado is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe otro tipo de vehículo con esa descripción.",
                    409);
            }

            tipoVehiculo.Descripcion =
                dto.Descripcion;

            tipoVehiculo.Activo =
                dto.Activo;

            tipoVehiculo.ActualizadoEn =
                DateTime.UtcNow;

            tipoVehiculo.ActualizadoPor =
                "SISTEMA";

            await _unidadTrabajo
                .CompletarAsync();

            return Respuesta.Correcta(
                MapearRespuesta(tipoVehiculo),
                "Tipo de vehículo actualizado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> EliminarAsync(
        int id)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del tipo de vehículo debe ser mayor que cero.",
                    400);
            }

            var tipoVehiculo =
                await _unidadTrabajo.TiposVehiculo
                    .ObtenerEntidadAsync(
                        tipo =>
                            tipo.TipoVehiculoId == id);

            if (tipoVehiculo is null)
            {
                return Respuesta.Fallida(
                    "El tipo de vehículo no existe.",
                    404);
            }

            if (!tipoVehiculo.Activo)
            {
                return Respuesta.Fallida(
                    "El tipo de vehículo ya se encuentra inactivo.",
                    409);
            }

            tipoVehiculo.Activo = false;

            tipoVehiculo.ActualizadoEn =
                DateTime.UtcNow;

            tipoVehiculo.ActualizadoPor =
                "SISTEMA";

            await _unidadTrabajo
                .CompletarAsync();

            return Respuesta.Correcta(
                MapearRespuesta(tipoVehiculo),
                "Tipo de vehículo desactivado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    private static TipoVehiculoRespuestaDto
        MapearRespuesta(
            TipoVehiculo tipoVehiculo)
    {
        return new TipoVehiculoRespuestaDto
        {
            TipoVehiculoId =
                tipoVehiculo.TipoVehiculoId,

            Descripcion =
                tipoVehiculo.Descripcion,

            Activo =
                tipoVehiculo.Activo,

            CreadoEn =
                tipoVehiculo.CreadoEn
        };
    }

    private static Respuesta CrearRespuestaError(
        Exception ex)
    {
        return Respuesta.Fallida(
            "Ocurrió un error al procesar el tipo de vehículo.",
            500,
            new List<string>
            {
                ex.Message
            });
    }
}