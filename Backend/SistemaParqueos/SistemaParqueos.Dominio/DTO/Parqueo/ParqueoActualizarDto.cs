using System.ComponentModel.DataAnnotations;

namespace SistemaParqueos.Dominio.DTO.Parqueo;

public class ParqueoActualizarDto
{
    [Required(ErrorMessage = "El nombre del parqueo es obligatorio.")]
    [StringLength(
        150,
        ErrorMessage = "El nombre no puede superar 150 caracteres.")]
    public string NombreParqueo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(
        250,
        ErrorMessage = "La dirección no puede superar 250 caracteres.")]
    public string Direccion { get; set; } = string.Empty;

    [StringLength(
        25,
        ErrorMessage = "El teléfono no puede superar 25 caracteres.")]
    public string? Telefono { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "La capacidad total debe ser mayor que cero.")]
    public int CapacidadTotal { get; set; }

    public bool Activo { get; set; } = true;
}