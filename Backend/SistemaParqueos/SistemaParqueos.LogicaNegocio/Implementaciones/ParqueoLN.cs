using SistemaParqueos.Dominio.DTO.Parqueo;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class ParqueoLN : IParqueoLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;

    public ParqueoLN(IUnidadTrabajoEF unidadTrabajo)
    {
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<Respuesta> ObtenerTodosAsync()
    {
        try
        {
            var parqueos =
                await _unidadTrabajo.Parqueos.ListarAsync();

            var resultado = parqueos
                .OrderBy(parqueo => parqueo.NombreParqueo)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Parqueos consultados correctamente.");
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
                    "El identificador del parqueo debe ser mayor que cero.",
                    400);
            }

            var parqueo =
                await _unidadTrabajo.Parqueos.ObtenerEntidadAsync(
                    registro => registro.ParqueoId == id);

            if (parqueo is null)
            {
                return Respuesta.Fallida(
                    "El parqueo solicitado no existe.",
                    404);
            }

            return Respuesta.Correcta(
                MapearRespuesta(parqueo),
                "Parqueo consultado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> AgregarAsync(
        ParqueoCrearDto parqueoDto)
    {
        try
        {
            NormalizarDatos(parqueoDto);

            if (string.IsNullOrWhiteSpace(
                parqueoDto.NombreParqueo))
            {
                return Respuesta.Fallida(
                    "El nombre del parqueo es obligatorio.",
                    400);
            }

            if (string.IsNullOrWhiteSpace(
                parqueoDto.Direccion))
            {
                return Respuesta.Fallida(
                    "La dirección es obligatoria.",
                    400);
            }

            if (parqueoDto.CapacidadTotal <= 0)
            {
                return Respuesta.Fallida(
                    "La capacidad total debe ser mayor que cero.",
                    400);
            }

            var parqueo = new Parqueo
            {
                NombreParqueo = parqueoDto.NombreParqueo,
                Direccion = parqueoDto.Direccion,
                Telefono = parqueoDto.Telefono,
                CapacidadTotal = parqueoDto.CapacidadTotal,
                Activo = true,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = "API"
            };

            await _unidadTrabajo.Parqueos.InsertarAsync(parqueo);
            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                MapearRespuesta(parqueo),
                "Parqueo registrado correctamente.",
                201);
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ActualizarAsync(
        int id,
        ParqueoActualizarDto parqueoDto)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del parqueo debe ser mayor que cero.",
                    400);
            }

            NormalizarDatos(parqueoDto);

            if (string.IsNullOrWhiteSpace(
                parqueoDto.NombreParqueo))
            {
                return Respuesta.Fallida(
                    "El nombre del parqueo es obligatorio.",
                    400);
            }

            if (string.IsNullOrWhiteSpace(
                parqueoDto.Direccion))
            {
                return Respuesta.Fallida(
                    "La dirección es obligatoria.",
                    400);
            }

            if (parqueoDto.CapacidadTotal <= 0)
            {
                return Respuesta.Fallida(
                    "La capacidad total debe ser mayor que cero.",
                    400);
            }

            var parqueoExistente =
                await _unidadTrabajo.Parqueos.ObtenerEntidadAsync(
                    registro => registro.ParqueoId == id);

            if (parqueoExistente is null)
            {
                return Respuesta.Fallida(
                    "El parqueo solicitado no existe.",
                    404);
            }

            var cantidadEspacios =
                await _unidadTrabajo.EspaciosParqueo.ContarAsync(
                    espacio =>
                        espacio.ParqueoId == id &&
                        espacio.Activo);

            if (parqueoDto.CapacidadTotal < cantidadEspacios)
            {
                return Respuesta.Fallida(
                    $"La capacidad no puede ser menor que los {cantidadEspacios} espacios activos existentes.",
                    409);
            }

            parqueoExistente.NombreParqueo =
                parqueoDto.NombreParqueo;

            parqueoExistente.Direccion =
                parqueoDto.Direccion;

            parqueoExistente.Telefono =
                parqueoDto.Telefono;

            parqueoExistente.CapacidadTotal =
                parqueoDto.CapacidadTotal;

            parqueoExistente.Activo =
                parqueoDto.Activo;

            parqueoExistente.ActualizadoEn =
                DateTime.UtcNow;

            parqueoExistente.ActualizadoPor =
                "API";

            _unidadTrabajo.Parqueos.Modificar(
                parqueoExistente);

            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                MapearRespuesta(parqueoExistente),
                "Parqueo actualizado correctamente.");
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
                    "El identificador del parqueo debe ser mayor que cero.",
                    400);
            }

            var parqueo =
                await _unidadTrabajo.Parqueos.ObtenerEntidadAsync(
                    registro => registro.ParqueoId == id);

            if (parqueo is null)
            {
                return Respuesta.Fallida(
                    "El parqueo solicitado no existe.",
                    404);
            }

            parqueo.Activo = false;
            parqueo.ActualizadoEn = DateTime.UtcNow;
            parqueo.ActualizadoPor = "API";

            _unidadTrabajo.Parqueos.Modificar(parqueo);
            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                true,
                "Parqueo desactivado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    private static ParqueoRespuestaDto MapearRespuesta(
        Parqueo parqueo)
    {
        return new ParqueoRespuestaDto
        {
            ParqueoId = parqueo.ParqueoId,
            NombreParqueo = parqueo.NombreParqueo,
            Direccion = parqueo.Direccion,
            Telefono = parqueo.Telefono,
            CapacidadTotal = parqueo.CapacidadTotal,
            Activo = parqueo.Activo,
            CreadoEn = parqueo.CreadoEn
        };
    }

    private static void NormalizarDatos(
        ParqueoCrearDto parqueoDto)
    {
        parqueoDto.NombreParqueo =
            parqueoDto.NombreParqueo.Trim();

        parqueoDto.Direccion =
            parqueoDto.Direccion.Trim();

        parqueoDto.Telefono =
            LimpiarOpcional(parqueoDto.Telefono);
    }

    private static void NormalizarDatos(
        ParqueoActualizarDto parqueoDto)
    {
        parqueoDto.NombreParqueo =
            parqueoDto.NombreParqueo.Trim();

        parqueoDto.Direccion =
            parqueoDto.Direccion.Trim();

        parqueoDto.Telefono =
            LimpiarOpcional(parqueoDto.Telefono);
    }

    private static string? LimpiarOpcional(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
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