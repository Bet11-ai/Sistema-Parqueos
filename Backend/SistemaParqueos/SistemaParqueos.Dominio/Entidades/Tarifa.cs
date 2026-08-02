using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaParqueos.Dominio.Entidades;

[Table("Tarifa")]
public partial class Tarifa
{
    [Key]
    public int TarifaId { get; set; }

    public int TipoVehiculoId { get; set; }

    [StringLength(100)]
    public string Descripcion { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoHora { get; set; }

    public bool Activo { get; set; }

    [Precision(3)]
    public DateTime CreadoEn { get; set; }

    [StringLength(50)]
    public string? CreadoPor { get; set; }

    [Precision(3)]
    public DateTime? ActualizadoEn { get; set; }

    [StringLength(50)]
    public string? ActualizadoPor { get; set; }

    public byte[] RowVer { get; set; } = null!;

    [ForeignKey("TipoVehiculoId")]
    [InverseProperty("Tarifas")]
    public virtual TipoVehiculo TipoVehiculo { get; set; } = null!;
}
