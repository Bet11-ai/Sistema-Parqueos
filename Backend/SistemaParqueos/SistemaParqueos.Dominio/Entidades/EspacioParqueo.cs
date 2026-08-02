using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaParqueos.Dominio.Entidades;

[Table("EspacioParqueo")]
[Index("ParqueoId", "NumeroEspacio", Name = "UQ_Espacio", IsUnique = true)]
public partial class EspacioParqueo
{
    [Key]
    public int EspacioId { get; set; }

    public int ParqueoId { get; set; }

    [StringLength(20)]
    public string NumeroEspacio { get; set; } = null!;

    public bool Disponible { get; set; }

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

    [InverseProperty("Espacio")]
    public virtual ICollection<IngresoVehiculo> IngresoVehiculos { get; set; } = new List<IngresoVehiculo>();

    [ForeignKey("ParqueoId")]
    [InverseProperty("EspacioParqueos")]
    public virtual Parqueo Parqueo { get; set; } = null!;
}
