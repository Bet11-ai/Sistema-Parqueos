using System.ComponentModel.DataAnnotations;

namespace SistemaParqueos.Dominio.DTO.Tarifa;

public class TarifaCrearDto
{
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Debe seleccionar un tipo de vehículo válido.")]
    public int TipoVehiculoId { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(
        100,
        ErrorMessage = "La descripción no puede superar 100 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(
        0.01,
        double.MaxValue,
        ErrorMessage = "El monto por hora debe ser mayor que cero.")]
    public decimal MontoHora { get; set; }
}