using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaParqueos.Dominio.Entidades;

[Table("IngresoVehiculo")]
public partial class IngresoVehiculo
{
    [Key]
    public int IngresoId { get; set; }

    public int VehiculoId { get; set; }

    public int EspacioId { get; set; }

    [Precision(3)]
    public DateTime FechaIngreso { get; set; }

    [Precision(3)]
    public DateTime? FechaSalida { get; set; }

    [StringLength(20)]
    public string Estado { get; set; } = null!;

    [Precision(3)]
    public DateTime CreadoEn { get; set; }

    [StringLength(50)]
    public string? CreadoPor { get; set; }

    [Precision(3)]
    public DateTime? ActualizadoEn { get; set; }

    [StringLength(50)]
    public string? ActualizadoPor { get; set; }

    public byte[] RowVer { get; set; } = null!;

    [ForeignKey("EspacioId")]
    [InverseProperty("IngresoVehiculos")]
    public virtual EspacioParqueo Espacio { get; set; } = null!;

    [InverseProperty("Ingreso")]
    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    [ForeignKey("VehiculoId")]
    [InverseProperty("IngresoVehiculos")]
    public virtual Vehiculo Vehiculo { get; set; } = null!;
}
