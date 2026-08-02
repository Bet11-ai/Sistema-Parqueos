using SistemaParqueos.Dominio.DTO.Vehiculo;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class VehiculoLN : IVehiculoLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;

    public VehiculoLN(IUnidadTrabajoEF unidadTrabajo)
    {
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<Respuesta> ObtenerTodosAsync()
    {
        try
        {
            var vehiculos =
                await _unidadTrabajo.Vehiculos.ListarAsync(
                    vehiculo => vehiculo.Cliente,
                    vehiculo => vehiculo.TipoVehiculo);

            var resultado = vehiculos
                .OrderBy(vehiculo => vehiculo.Placa)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Vehículos consultados correctamente.");
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
                    "El identificador del vehículo debe ser mayor que cero.",
                    400);
            }

            var vehiculo =
                await _unidadTrabajo.Vehiculos
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.VehiculoId == id,
                        registro => registro.Cliente,
                        registro => registro.TipoVehiculo);

            if (vehiculo is null)
            {
                return Respuesta.Fallida(
                    "El vehículo solicitado no existe.",
                    404);
            }

            return Respuesta.Correcta(
                MapearRespuesta(vehiculo),
                "Vehículo consultado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> AgregarAsync(
        VehiculoCrearDto vehiculoDto)
    {
        try
        {
            NormalizarDatos(vehiculoDto);

            var validacionRelaciones =
                await ValidarRelacionesAsync(
                    vehiculoDto.ClienteId,
                    vehiculoDto.TipoVehiculoId);

            if (validacionRelaciones is not null)
            {
                return validacionRelaciones;
            }

            var placaDuplicada =
                await _unidadTrabajo.Vehiculos
                    .ObtenerEntidadAsync(registro =>
                        registro.Placa == vehiculoDto.Placa);

            if (placaDuplicada is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe un vehículo registrado con esa placa.",
                    409);
            }

            var vehiculo = new Vehiculo
            {
                ClienteId = vehiculoDto.ClienteId,
                TipoVehiculoId = vehiculoDto.TipoVehiculoId,
                Placa = vehiculoDto.Placa,
                Marca = vehiculoDto.Marca,
                Modelo = vehiculoDto.Modelo,
                Color = vehiculoDto.Color,
                Activo = true,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = "API"
            };

            await _unidadTrabajo.Vehiculos
                .InsertarAsync(vehiculo);

            await _unidadTrabajo.CompletarAsync();

            var vehiculoCreado =
                await _unidadTrabajo.Vehiculos
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.VehiculoId ==
                            vehiculo.VehiculoId,
                        registro => registro.Cliente,
                        registro => registro.TipoVehiculo);

            return Respuesta.Correcta(
                MapearRespuesta(vehiculoCreado!),
                "Vehículo registrado correctamente.",
                201);
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ActualizarAsync(
        int id,
        VehiculoActualizarDto vehiculoDto)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del vehículo debe ser mayor que cero.",
                    400);
            }

            NormalizarDatos(vehiculoDto);

            var vehiculoExistente =
                await _unidadTrabajo.Vehiculos
                    .ObtenerEntidadAsync(registro =>
                        registro.VehiculoId == id);

            if (vehiculoExistente is null)
            {
                return Respuesta.Fallida(
                    "El vehículo solicitado no existe.",
                    404);
            }

            var validacionRelaciones =
                await ValidarRelacionesAsync(
                    vehiculoDto.ClienteId,
                    vehiculoDto.TipoVehiculoId);

            if (validacionRelaciones is not null)
            {
                return validacionRelaciones;
            }

            var placaDuplicada =
                await _unidadTrabajo.Vehiculos
                    .ObtenerEntidadAsync(registro =>
                        registro.Placa == vehiculoDto.Placa &&
                        registro.VehiculoId != id);

            if (placaDuplicada is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe otro vehículo registrado con esa placa.",
                    409);
            }

            vehiculoExistente.ClienteId =
                vehiculoDto.ClienteId;

            vehiculoExistente.TipoVehiculoId =
                vehiculoDto.TipoVehiculoId;

            vehiculoExistente.Placa =
                vehiculoDto.Placa;

            vehiculoExistente.Marca =
                vehiculoDto.Marca;

            vehiculoExistente.Modelo =
                vehiculoDto.Modelo;

            vehiculoExistente.Color =
                vehiculoDto.Color;

            vehiculoExistente.Activo =
                vehiculoDto.Activo;

            vehiculoExistente.ActualizadoEn =
                DateTime.UtcNow;

            vehiculoExistente.ActualizadoPor =
                "API";

            _unidadTrabajo.Vehiculos
                .Modificar(vehiculoExistente);

            await _unidadTrabajo.CompletarAsync();

            var vehiculoActualizado =
                await _unidadTrabajo.Vehiculos
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.VehiculoId == id,
                        registro => registro.Cliente,
                        registro => registro.TipoVehiculo);

            return Respuesta.Correcta(
                MapearRespuesta(vehiculoActualizado!),
                "Vehículo actualizado correctamente.");
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
                    "El identificador del vehículo debe ser mayor que cero.",
                    400);
            }

            var vehiculo =
                await _unidadTrabajo.Vehiculos
                    .ObtenerEntidadAsync(registro =>
                        registro.VehiculoId == id);

            if (vehiculo is null)
            {
                return Respuesta.Fallida(
                    "El vehículo solicitado no existe.",
                    404);
            }

            vehiculo.Activo = false;
            vehiculo.ActualizadoEn = DateTime.UtcNow;
            vehiculo.ActualizadoPor = "API";

            _unidadTrabajo.Vehiculos.Modificar(vehiculo);
            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                true,
                "Vehículo desactivado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    private async Task<Respuesta?> ValidarRelacionesAsync(
        int clienteId,
        int tipoVehiculoId)
    {
        var cliente =
            await _unidadTrabajo.Clientes
                .ObtenerEntidadAsync(registro =>
                    registro.ClienteId == clienteId);

        if (cliente is null)
        {
            return Respuesta.Fallida(
                "El cliente seleccionado no existe.",
                400);
        }

        if (!cliente.Activo)
        {
            return Respuesta.Fallida(
                "El cliente seleccionado está inactivo.",
                409);
        }

        var tipoVehiculo =
            await _unidadTrabajo.TiposVehiculo
                .ObtenerEntidadAsync(registro =>
                    registro.TipoVehiculoId ==
                    tipoVehiculoId);

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

        return null;
    }

    private static VehiculoRespuestaDto MapearRespuesta(
        Vehiculo vehiculo)
    {
        return new VehiculoRespuestaDto
        {
            VehiculoId = vehiculo.VehiculoId,
            ClienteId = vehiculo.ClienteId,
            NombreCliente =
                $"{vehiculo.Cliente.Nombre} " +
                $"{vehiculo.Cliente.Apellidos}",
            TipoVehiculoId = vehiculo.TipoVehiculoId,
            TipoVehiculo =
                vehiculo.TipoVehiculo.Descripcion,
            Placa = vehiculo.Placa,
            Marca = vehiculo.Marca,
            Modelo = vehiculo.Modelo,
            Color = vehiculo.Color,
            Activo = vehiculo.Activo,
            CreadoEn = vehiculo.CreadoEn
        };
    }

    private static void NormalizarDatos(
        VehiculoCrearDto vehiculoDto)
    {
        vehiculoDto.Placa =
            vehiculoDto.Placa
                .Trim()
                .ToUpperInvariant();

        vehiculoDto.Marca =
            vehiculoDto.Marca.Trim();

        vehiculoDto.Modelo =
            LimpiarOpcional(vehiculoDto.Modelo);

        vehiculoDto.Color =
            LimpiarOpcional(vehiculoDto.Color);
    }

    private static void NormalizarDatos(
        VehiculoActualizarDto vehiculoDto)
    {
        vehiculoDto.Placa =
            vehiculoDto.Placa
                .Trim()
                .ToUpperInvariant();

        vehiculoDto.Marca =
            vehiculoDto.Marca.Trim();

        vehiculoDto.Modelo =
            LimpiarOpcional(vehiculoDto.Modelo);

        vehiculoDto.Color =
            LimpiarOpcional(vehiculoDto.Color);
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