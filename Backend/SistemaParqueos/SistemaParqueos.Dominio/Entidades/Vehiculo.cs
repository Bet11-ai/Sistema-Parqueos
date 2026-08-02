using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SistemaParqueos.Dominio.Entidades;

namespace SistemaParqueos.Dominio.Entidades;

[Table("Vehiculo")]
[Index("Placa", Name = "UQ_Vehiculo_Placa", IsUnique = true)]
public partial class Vehiculo
{
    [Key]
    public int VehiculoId { get; set; }

    public int ClienteId { get; set; }

    public int TipoVehiculoId { get; set; }

    [StringLength(20)]
    public string Placa { get; set; } = null!;

    [StringLength(100)]
    public string Marca { get; set; } = null!;

    [StringLength(100)]
    public string? Modelo { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

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

    [ForeignKey("ClienteId")]
    [InverseProperty("Vehiculos")]
    public virtual Cliente Cliente { get; set; } = null!;

    [InverseProperty("Vehiculo")]
    public virtual ICollection<IngresoVehiculo> IngresoVehiculos { get; set; } = new List<IngresoVehiculo>();

    [ForeignKey("TipoVehiculoId")]
    [InverseProperty("Vehiculos")]
    public virtual TipoVehiculo TipoVehiculo { get; set; } = null!;
}
