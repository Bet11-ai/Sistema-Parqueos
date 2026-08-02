using System.ComponentModel.DataAnnotations;

namespace SistemaParqueos.Dominio.DTO.IngresoVehiculo;

public class IngresoVehiculoCrearDto
{
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Debe seleccionar un vehículo válido.")]
    public int VehiculoId { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Debe seleccionar un espacio válido.")]
    public int EspacioId { get; set; }
}