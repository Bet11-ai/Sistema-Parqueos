using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaParqueos.Dominio.Entidades;

[Table("TipoVehiculo")]
[Index("Descripcion", Name = "UQ_TipoVehiculo", IsUnique = true)]
public partial class TipoVehiculo
{
    [Key]
    public int TipoVehiculoId { get; set; }

    [StringLength(100)]
    public string Descripcion { get; set; } = null!;

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

    [InverseProperty("TipoVehiculo")]
    public virtual ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();

    [InverseProperty("TipoVehiculo")]
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
