using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaParqueos.Dominio.Entidades;

[Table("Cliente")]
[Index("Cedula", Name = "UQ_Cliente_Cedula", IsUnique = true)]
public partial class Cliente
{
    [Key]
    public int ClienteId { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(150)]
    public string Apellidos { get; set; } = null!;

    [StringLength(25)]
    public string Cedula { get; set; } = null!;

    [StringLength(25)]
    public string? Telefono { get; set; }

    [StringLength(254)]
    public string? Correo { get; set; }

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

    [InverseProperty("Cliente")]
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
