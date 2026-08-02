using System.ComponentModel.DataAnnotations;

namespace SistemaParqueos.Dominio.DTO.EspacioParqueo;

public class EspacioParqueoCrearDto
{
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Debe seleccionar un parqueo válido.")]
    public int ParqueoId { get; set; }

    [Required(
        ErrorMessage = "El número de espacio es obligatorio.")]
    [StringLength(
        20,
        ErrorMessage = "El número de espacio no puede superar 20 caracteres.")]
    public string NumeroEspacio { get; set; } = string.Empty;

    public bool Disponible { get; set; } = true;
}