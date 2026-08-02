using System.ComponentModel.DataAnnotations;

namespace SistemaParqueos.Dominio.DTO.TipoVehiculo;

public class TipoVehiculoCrearDto
{
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(100,
        ErrorMessage = "La descripción no puede superar los 100 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;
}