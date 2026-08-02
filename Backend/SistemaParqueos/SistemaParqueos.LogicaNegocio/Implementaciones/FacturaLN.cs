using SistemaParqueos.Dominio.DTO.Factura;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class FacturaLN : IFacturaLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;

    public FacturaLN(
        IUnidadTrabajoEF unidadTrabajo)
    {
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<Respuesta> ObtenerTodasAsync()
    {
        try
        {
            var facturas =
                await _unidadTrabajo.Facturas
                    .ListarAsync(
                        factura => factura.Ingreso,
                        factura => factura.Ingreso.Vehiculo,
                        factura =>
                            factura.Ingreso.Vehiculo.TipoVehiculo,
                        factura => factura.Ingreso.Espacio,
                        factura =>
                            factura.Ingreso.Espacio.Parqueo);

            var resultado = facturas
                .OrderByDescending(factura =>
                    factura.FechaFactura)
                .Select(MapearRespuesta)
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Facturas consultadas correctamente.");
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
                    "El identificador de la factura debe ser mayor que cero.",
                    400);
            }

            var factura =
                await _unidadTrabajo.Facturas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.FacturaId == id,
                        registro => registro.Ingreso,
                        registro =>
                            registro.Ingreso.Vehiculo,
                        registro =>
                            registro.Ingreso.Vehiculo.TipoVehiculo,
                        registro =>
                            registro.Ingreso.Espacio,
                        registro =>
                            registro.Ingreso.Espacio.Parqueo);

            if (factura is null)
            {
                return Respuesta.Fallida(
                    "La factura solicitada no existe.",
                    404);
            }

            return Respuesta.Correcta(
                MapearRespuesta(factura),
                "Factura consultada correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ObtenerPorIngresoAsync(
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
                            registro.IngresoId == ingresoId);

            if (ingreso is null)
            {
                return Respuesta.Fallida(
                    "El ingreso solicitado no existe.",
                    404);
            }

            var factura =
                await _unidadTrabajo.Facturas
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.IngresoId == ingresoId,
                        registro => registro.Ingreso,
                        registro =>
                            registro.Ingreso.Vehiculo,
                        registro =>
                            registro.Ingreso.Vehiculo.TipoVehiculo,
                        registro =>
                            registro.Ingreso.Espacio,
                        registro =>
                            registro.Ingreso.Espacio.Parqueo);

            if (factura is null)
            {
                return Respuesta.Fallida(
                    "No existe una factura para el ingreso indicado.",
                    404);
            }

            return Respuesta.Correcta(
                MapearRespuesta(factura),
                "Factura consultada correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    private static FacturaRespuestaDto MapearRespuesta(
        Factura factura)
    {
        return new FacturaRespuestaDto
        {
            FacturaId =
                factura.FacturaId,

            IngresoId =
                factura.IngresoId,

            Placa =
                factura.Ingreso.Vehiculo.Placa,

            TipoVehiculo =
                factura.Ingreso.Vehiculo
                    .TipoVehiculo.Descripcion,

            NumeroEspacio =
                factura.Ingreso.Espacio.NumeroEspacio,

            NombreParqueo =
                factura.Ingreso.Espacio
                    .Parqueo.NombreParqueo,

            FechaIngreso =
                factura.Ingreso.FechaIngreso,

            FechaSalida =
                factura.Ingreso.FechaSalida,

            FechaFactura =
                factura.FechaFactura,

            HorasCobradas =
                factura.HorasCobradas,

            MontoTotal =
                factura.MontoTotal
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