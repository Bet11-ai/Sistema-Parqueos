using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaParqueos.Dominio.Entidades;

[Table("Factura")]
public partial class Factura
{
    [Key]
    public int FacturaId { get; set; }

    public int IngresoId { get; set; }

    [Precision(3)]
    public DateTime FechaFactura { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal HorasCobradas { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoTotal { get; set; }

    [Precision(3)]
    public DateTime CreadoEn { get; set; }

    [StringLength(50)]
    public string? CreadoPor { get; set; }

    [Precision(3)]
    public DateTime? ActualizadoEn { get; set; }

    [StringLength(50)]
    public string? ActualizadoPor { get; set; }

    public byte[] RowVer { get; set; } = null!;

    [ForeignKey("IngresoId")]
    [InverseProperty("Facturas")]
    public virtual IngresoVehiculo Ingreso { get; set; } = null!;
}
