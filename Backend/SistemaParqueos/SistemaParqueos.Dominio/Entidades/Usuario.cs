namespace SistemaParqueos.Dominio.Entidades;

public class Usuario
{
    public int UsuarioId { get; set; }

    public int RolId { get; set; }

    public string NombreCompleto { get; set; }
        = string.Empty;

    public string Correo { get; set; }
        = string.Empty;

    public string ContrasenaHash { get; set; }
        = string.Empty;

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }

    public string? CreadoPor { get; set; }

    public DateTime? ActualizadoEn { get; set; }

    public string? ActualizadoPor { get; set; }

    public virtual Rol Rol { get; set; } = null!;
}