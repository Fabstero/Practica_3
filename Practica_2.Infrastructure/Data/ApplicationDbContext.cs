using Microsoft.EntityFrameworkCore;
using Practica_2.Domain.Entidades;

namespace Practica_2.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Votante> Votantes { get; set; }

    public virtual DbSet<PartidoPolitico> PartidosPoliticos { get; set; }

    public virtual DbSet<Voto> Votos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=votaciones.db;Foreign Keys=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Votante>(entity =>
        {
            entity.ToTable("Votantes", t =>
                t.HasCheckConstraint("CK_Votantes_HaVotado", "HaVotado IN (0, 1)"));

            entity.HasKey(e => e.IdVotante);

            entity.Property(e => e.IdVotante)
                .HasColumnName("IdVotante")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Cedula)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasColumnName("Cedula");

            entity.Property(e => e.NombreCompleto)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasColumnName("NombreCompleto");

            entity.Property(e => e.FechaNacimiento)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasColumnName("FechaNacimiento");

            entity.Property(e => e.Provincia)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasColumnName("Provincia");

            entity.Property(e => e.HaVotado)
                .IsRequired()
                .HasColumnType("INTEGER")
                .HasDefaultValueSql("0")
                .ValueGeneratedNever()
                .HasColumnName("HaVotado");

            entity.HasIndex(e => e.Cedula, "IX_Votantes_Cedula")
                .IsUnique();
        });

        modelBuilder.Entity<PartidoPolitico>(entity =>
        {
            entity.ToTable("PartidosPoliticos", t =>
                t.HasCheckConstraint("CK_PartidosPoliticos_Activo", "Activo IN (0, 1)"));

            entity.HasKey(e => e.IdPartido);

            entity.Property(e => e.IdPartido)
                .HasColumnName("IdPartido")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasColumnName("Nombre");

            entity.Property(e => e.Siglas)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasColumnName("Siglas");

            entity.Property(e => e.BanderaUrl)
                .IsRequired(false)
                .HasColumnType("TEXT")
                .HasColumnName("BanderaUrl");

            entity.Property(e => e.NombreCandidato)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasColumnName("NombreCandidato");

            entity.Property(e => e.Activo)
                .IsRequired()
                .HasColumnType("INTEGER")
                .HasDefaultValueSql("1")
                .ValueGeneratedNever()
                .HasColumnName("Activo");

            entity.HasIndex(e => e.Nombre, "UQ_PartidosPoliticos_Nombre")
                .IsUnique();

            entity.HasIndex(e => e.Siglas, "UQ_PartidosPoliticos_Siglas")
                .IsUnique();
        });

        modelBuilder.Entity<Voto>(entity =>
        {
            entity.ToTable("Votos");

            entity.HasKey(e => e.IdVoto);

            entity.Property(e => e.IdVoto)
                .HasColumnName("IdVoto")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.IdVotante)
                .IsRequired()
                .HasColumnName("IdVotante");

            entity.Property(e => e.IdPartido)
                .IsRequired()
                .HasColumnName("IdPartido");

            entity.Property(e => e.FechaHoraVoto)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .ValueGeneratedOnAdd()
                .HasColumnName("FechaHoraVoto");

            entity.HasIndex(e => e.IdVotante, "UQ_Votos_IdVotante")
                .IsUnique();

            entity.HasIndex(e => e.IdPartido, "IX_Votos_IdPartido");

            entity.HasOne(d => d.Votante)
                .WithOne(v => v.Voto)
                .HasForeignKey<Voto>(d => d.IdVotante)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.PartidoPolitico)
                .WithMany(p => p.Votos)
                .HasForeignKey(d => d.IdPartido)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
