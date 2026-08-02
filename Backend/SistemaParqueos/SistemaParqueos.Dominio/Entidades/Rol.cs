namespace SistemaParqueos.Dominio.Entidades;

public class Rol
{
    public int RolId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }

    public string? CreadoPor { get; set; }

    public DateTime? ActualizadoEn { get; set; }

    public string? ActualizadoPor { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; }
        = new List<Usuario>();
}