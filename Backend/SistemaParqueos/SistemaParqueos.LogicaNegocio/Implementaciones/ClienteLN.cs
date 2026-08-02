using SistemaParqueos.Dominio.DTO.Cliente;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class ClienteLN : IClienteLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;

    public ClienteLN(IUnidadTrabajoEF unidadTrabajo)
    {
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<Respuesta> ObtenerTodosAsync()
    {
        try
        {
            var clientes =
                await _unidadTrabajo.Clientes.ListarAsync();

            var resultado = clientes
                .OrderBy(cliente => cliente.Nombre)
                .ThenBy(cliente => cliente.Apellidos)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Clientes consultados correctamente.");
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
                    "El identificador del cliente debe ser mayor que cero.",
                    400);
            }

            var cliente =
                await _unidadTrabajo.Clientes.ObtenerEntidadAsync(
                    registro => registro.ClienteId == id);

            if (cliente is null)
            {
                return Respuesta.Fallida(
                    "El cliente solicitado no existe.",
                    404);
            }

            return Respuesta.Correcta(
                MapearRespuesta(cliente),
                "Cliente consultado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> AgregarAsync(
        ClienteCrearDto clienteDto)
    {
        try
        {
            NormalizarDatos(clienteDto);

            if (string.IsNullOrWhiteSpace(clienteDto.Nombre))
            {
                return Respuesta.Fallida(
                    "El nombre es obligatorio.",
                    400);
            }

            if (string.IsNullOrWhiteSpace(clienteDto.Apellidos))
            {
                return Respuesta.Fallida(
                    "Los apellidos son obligatorios.",
                    400);
            }

            if (string.IsNullOrWhiteSpace(clienteDto.Cedula))
            {
                return Respuesta.Fallida(
                    "La cédula es obligatoria.",
                    400);
            }

            var clienteDuplicado =
                await _unidadTrabajo.Clientes.ObtenerEntidadAsync(
                    registro =>
                        registro.Cedula == clienteDto.Cedula);

            if (clienteDuplicado is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe un cliente registrado con esa cédula.",
                    409);
            }

            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                Apellidos = clienteDto.Apellidos,
                Cedula = clienteDto.Cedula,
                Telefono = clienteDto.Telefono,
                Correo = clienteDto.Correo,
                Activo = true,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = "API"
            };

            await _unidadTrabajo.Clientes.InsertarAsync(cliente);
            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                MapearRespuesta(cliente),
                "Cliente registrado correctamente.",
                201);
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ActualizarAsync(
        int id,
        ClienteActualizarDto clienteDto)
    {
        try
        {
            if (id <= 0)
            {
                return Respuesta.Fallida(
                    "El identificador del cliente debe ser mayor que cero.",
                    400);
            }

            NormalizarDatos(clienteDto);

            var clienteExistente =
                await _unidadTrabajo.Clientes.ObtenerEntidadAsync(
                    registro => registro.ClienteId == id);

            if (clienteExistente is null)
            {
                return Respuesta.Fallida(
                    "El cliente solicitado no existe.",
                    404);
            }

            var cedulaDuplicada =
                await _unidadTrabajo.Clientes.ObtenerEntidadAsync(
                    registro =>
                        registro.Cedula == clienteDto.Cedula &&
                        registro.ClienteId != id);

            if (cedulaDuplicada is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe otro cliente registrado con esa cédula.",
                    409);
            }

            clienteExistente.Nombre = clienteDto.Nombre;
            clienteExistente.Apellidos = clienteDto.Apellidos;
            clienteExistente.Cedula = clienteDto.Cedula;
            clienteExistente.Telefono = clienteDto.Telefono;
            clienteExistente.Correo = clienteDto.Correo;
            clienteExistente.Activo = clienteDto.Activo;
            clienteExistente.ActualizadoEn = DateTime.UtcNow;
            clienteExistente.ActualizadoPor = "API";

            _unidadTrabajo.Clientes.Modificar(clienteExistente);
            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                MapearRespuesta(clienteExistente),
                "Cliente actualizado correctamente.");
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
                    "El identificador del cliente debe ser mayor que cero.",
                    400);
            }

            var cliente =
                await _unidadTrabajo.Clientes.ObtenerEntidadAsync(
                    registro => registro.ClienteId == id);

            if (cliente is null)
            {
                return Respuesta.Fallida(
                    "El cliente solicitado no existe.",
                    404);
            }

            cliente.Activo = false;
            cliente.ActualizadoEn = DateTime.UtcNow;
            cliente.ActualizadoPor = "API";

            _unidadTrabajo.Clientes.Modificar(cliente);
            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                true,
                "Cliente desactivado correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    private static ClienteRespuestaDto MapearRespuesta(
        Cliente cliente)
    {
        return new ClienteRespuestaDto
        {
            ClienteId = cliente.ClienteId,
            Nombre = cliente.Nombre,
            Apellidos = cliente.Apellidos,
            Cedula = cliente.Cedula,
            Telefono = cliente.Telefono,
            Correo = cliente.Correo,
            Activo = cliente.Activo,
            CreadoEn = cliente.CreadoEn
        };
    }

    private static void NormalizarDatos(
        ClienteCrearDto clienteDto)
    {
        clienteDto.Nombre =
            clienteDto.Nombre.Trim();

        clienteDto.Apellidos =
            clienteDto.Apellidos.Trim();

        clienteDto.Cedula =
            clienteDto.Cedula.Trim();

        clienteDto.Telefono =
            LimpiarOpcional(clienteDto.Telefono);

        clienteDto.Correo =
            LimpiarOpcional(clienteDto.Correo)?
                .ToLowerInvariant();
    }

    private static void NormalizarDatos(
        ClienteActualizarDto clienteDto)
    {
        clienteDto.Nombre =
            clienteDto.Nombre.Trim();

        clienteDto.Apellidos =
            clienteDto.Apellidos.Trim();

        clienteDto.Cedula =
            clienteDto.Cedula.Trim();

        clienteDto.Telefono =
            LimpiarOpcional(clienteDto.Telefono);

        clienteDto.Correo =
            LimpiarOpcional(clienteDto.Correo)?
                .ToLowerInvariant();
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