using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SistemaParqueos.Dominio.Entidades;


namespace SistemaParqueos.AccesoDatos.Contexto;


public partial class ParqueosDbContext : DbContext
{
    public ParqueosDbContext()
    {
    }

    public ParqueosDbContext(DbContextOptions<ParqueosDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<EspacioParqueo> EspacioParqueos { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<IngresoVehiculo> IngresoVehiculos { get; set; }

    public virtual DbSet<Parqueo> Parqueos { get; set; }

    public virtual DbSet<Tarifa> Tarifas { get; set; }

    public virtual DbSet<TipoVehiculo> TipoVehiculos { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //    => optionsBuilder.UseSqlServer("Server=BETQUIROS\\SQLEXPRESS;Database=ParqueosDB;User Id=Andre;Password=tool;TrustServerCertificate=True;")*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.RolId);

            entity.ToTable("Rol");

            entity.HasIndex(e => e.Nombre)
                .IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50);

            entity.Property(e => e.Descripcion)
                .HasMaxLength(150);

            entity.Property(e => e.CreadoPor)
                .HasMaxLength(100);

            entity.Property(e => e.ActualizadoPor)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId);

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Correo)
                .IsUnique();

            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(150);

            entity.Property(e => e.Correo)
                .HasMaxLength(254);

            entity.Property(e => e.ContrasenaHash)
                .HasMaxLength(500);

            entity.Property(e => e.CreadoPor)
                .HasMaxLength(100);

            entity.Property(e => e.ActualizadoPor)
                .HasMaxLength(100);

            entity.HasOne(e => e.Rol)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(e => e.RolId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Usuario_Rol");
        });












        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<EspacioParqueo>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Disponible).HasDefaultValue(true);
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Parqueo).WithMany(p => p.EspacioParqueos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Espacio_Parqueo");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Ingreso).WithMany(p => p.Facturas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factura_Ingreso");
        });

        modelBuilder.Entity<IngresoVehiculo>(entity =>
        {
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Espacio).WithMany(p => p.IngresoVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ingreso_Espacio");

            entity.HasOne(d => d.Vehiculo).WithMany(p => p.IngresoVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ingreso_Vehiculo");
        });

        modelBuilder.Entity<Parqueo>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.TipoVehiculo).WithMany(p => p.Tarifas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarifa_TipoVehiculo");
        });

        modelBuilder.Entity<TipoVehiculo>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Cliente).WithMany(p => p.Vehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehiculo_Cliente");

            entity.HasOne(d => d.TipoVehiculo).WithMany(p => p.Vehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehiculo_TipoVehiculo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
