using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaParqueos.Dominio.Entidades;

[Table("Parqueo")]
public partial class Parqueo
{
    [Key]
    public int ParqueoId { get; set; }

    [StringLength(150)]
    public string NombreParqueo { get; set; } = null!;

    [StringLength(250)]
    public string Direccion { get; set; } = null!;

    [StringLength(25)]
    public string? Telefono { get; set; }

    public int CapacidadTotal { get; set; }

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

    [InverseProperty("Parqueo")]
    public virtual ICollection<EspacioParqueo> EspacioParqueos { get; set; } = new List<EspacioParqueo>();
}
