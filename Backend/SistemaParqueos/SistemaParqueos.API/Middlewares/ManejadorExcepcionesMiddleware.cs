using Microsoft.EntityFrameworkCore;
using SistemaParqueos.Utilitarios;
using System.Net;
using System.Text.Json;

namespace SistemaParqueos.API.Middlewares;

public class ManejadorExcepcionesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ManejadorExcepcionesMiddleware> _logger;

    public ManejadorExcepcionesMiddleware(
        RequestDelegate next,
        ILogger<ManejadorExcepcionesMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _next(contexto);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Error al actualizar la base de datos.");

            await EscribirRespuestaAsync(
                contexto,
                Respuesta.Fallida(
                    "No fue posible guardar los cambios en la base de datos.",
                    StatusCodes.Status409Conflict,
                    new List<string>
                    {
                        ObtenerMensajeBase(ex)
                    }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(
                ex,
                "Solicitud con argumentos inválidos.");

            await EscribirRespuestaAsync(
                contexto,
                Respuesta.Fallida(
                    ex.Message,
                    StatusCodes.Status400BadRequest));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "La operación solicitada no es válida.");

            await EscribirRespuestaAsync(
                contexto,
                Respuesta.Fallida(
                    ex.Message,
                    StatusCodes.Status409Conflict));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "No se encontró el recurso solicitado.");

            await EscribirRespuestaAsync(
                contexto,
                Respuesta.Fallida(
                    ex.Message,
                    StatusCodes.Status404NotFound));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ocurrió un error interno no controlado.");

            await EscribirRespuestaAsync(
                contexto,
                Respuesta.Fallida(
                    "Ocurrió un error interno al procesar la solicitud.",
                    StatusCodes.Status500InternalServerError));
        }
    }

    private static async Task EscribirRespuestaAsync(
        HttpContext contexto,
        Respuesta respuesta)
    {
        contexto.Response.StatusCode =
            respuesta.CodigoEstado;

        contexto.Response.ContentType =
            "application/json; charset=utf-8";

        var opcionesJson = new JsonSerializerOptions
        {
            PropertyNamingPolicy =
                JsonNamingPolicy.CamelCase
        };

        var json =
            JsonSerializer.Serialize(
                respuesta,
                opcionesJson);

        await contexto.Response.WriteAsync(json);
    }

    private static string ObtenerMensajeBase(
        Exception excepcion)
    {
        var actual = excepcion;

        while (actual.InnerException is not null)
        {
            actual = actual.InnerException;
        }

        return actual.Message;
    }
}