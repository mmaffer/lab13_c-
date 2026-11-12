using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Models;

public partial class VeterinariaContext : DbContext
{
    public VeterinariaContext()
    {
    }

    public VeterinariaContext(DbContextOptions<VeterinariaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mascota> Mascotas { get; set; }

    public virtual DbSet<Propietario> Propietarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasKey(e => e.IdMascota).HasName("mascota_pkey");

            entity.ToTable("mascota");

            entity.HasIndex(e => e.IdPropietario, "ix_mascota_id_propietario");

            entity.HasIndex(e => e.Microchip, "mascota_microchip_key").IsUnique();

            entity.Property(e => e.IdMascota)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_mascota");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Especie)
                .HasMaxLength(30)
                .HasColumnName("especie");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.IdPropietario).HasColumnName("id_propietario");
            entity.Property(e => e.Microchip)
                .HasMaxLength(20)
                .HasColumnName("microchip");
            entity.Property(e => e.Nombre)
                .HasMaxLength(60)
                .HasColumnName("nombre");
            entity.Property(e => e.PesoKg)
                .HasPrecision(5, 2)
                .HasColumnName("peso_kg");
            entity.Property(e => e.Raza)
                .HasMaxLength(60)
                .HasColumnName("raza");
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("sexo");

            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.Mascotas)
                .HasForeignKey(d => d.IdPropietario)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("mascota_id_propietario_fkey");
        });

        modelBuilder.Entity<Propietario>(entity =>
        {
            entity.HasKey(e => e.IdPropietario).HasName("propietario_pkey");

            entity.ToTable("propietario");

            entity.HasIndex(e => e.Documento, "propietario_documento_key").IsUnique();

            entity.Property(e => e.IdPropietario)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_propietario");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(80)
                .HasColumnName("apellidos");
            entity.Property(e => e.Direccion)
                .HasMaxLength(150)
                .HasColumnName("direccion");
            entity.Property(e => e.Documento)
                .HasMaxLength(15)
                .HasColumnName("documento");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Nombres)
                .HasMaxLength(80)
                .HasColumnName("nombres");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
