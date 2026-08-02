using System.ComponentModel.DataAnnotations;

namespace SistemaParqueos.Dominio.DTO.Vehiculo;

public class VehiculoCrearDto
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Debe seleccionar un cliente válido.")]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Debe seleccionar un tipo de vehículo válido.")]
    public int TipoVehiculoId { get; set; }

    [Required(ErrorMessage = "La placa es obligatoria.")]
    [StringLength(20,
        ErrorMessage = "La placa no puede superar 20 caracteres.")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(100,
        ErrorMessage = "La marca no puede superar 100 caracteres.")]
    public string Marca { get; set; } = string.Empty;

    [StringLength(100,
        ErrorMessage = "El modelo no puede superar 100 caracteres.")]
    public string? Modelo { get; set; }

    [StringLength(50,
        ErrorMessage = "El color no puede superar 50 caracteres.")]
    public string? Color { get; set; }
}