namespace SistemaParqueos.Utilitarios;

public class Respuesta
{
    public bool Exito { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public object? ValorRetorno { get; set; }

    public List<string>? Errores { get; set; }

    public int CodigoEstado { get; set; }

    public static Respuesta Correcta(
        object? valorRetorno,
        string mensaje,
        int codigoEstado = 200)
    {
        return new Respuesta
        {
            Exito = true,
            Mensaje = mensaje,
            ValorRetorno = valorRetorno,
            Errores = null,
            CodigoEstado = codigoEstado
        };
    }

    public static Respuesta Fallida(
        string mensaje,
        int codigoEstado,
        List<string>? errores = null)
    {
        return new Respuesta
        {
            Exito = false,
            Mensaje = mensaje,
            ValorRetorno = null,
            Errores = errores,
            CodigoEstado = codigoEstado
        };
    }
}