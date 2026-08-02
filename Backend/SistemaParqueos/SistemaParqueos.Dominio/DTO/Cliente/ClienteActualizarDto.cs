using System.ComponentModel.DataAnnotations;

namespace SistemaParqueos.Dominio.DTO.Cliente;

public class ClienteActualizarDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(150, ErrorMessage = "Los apellidos no pueden superar 150 caracteres.")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [StringLength(25, ErrorMessage = "La cédula no puede superar 25 caracteres.")]
    public string Cedula { get; set; } = string.Empty;

    [StringLength(25, ErrorMessage = "El teléfono no puede superar 25 caracteres.")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    [StringLength(254, ErrorMessage = "El correo no puede superar 254 caracteres.")]
    public string? Correo { get; set; }

    public bool Activo { get; set; } = true;
}