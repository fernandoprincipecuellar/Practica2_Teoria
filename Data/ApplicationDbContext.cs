using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Practica2_Teoria.Models;

namespace Practica2_Teoria.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Curso>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.Codigo).IsUnique();
            entity.Property(c => c.Codigo).IsRequired();
            entity.Property(c => c.Nombre).IsRequired();
            entity.Property(c => c.Creditos).IsRequired();
            entity.Property(c => c.HorarioInicio).IsRequired();
            entity.Property(c => c.HorarioFin).IsRequired();
            entity.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0");
                tb.HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");
            });
        });

        builder.Entity<Matricula>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => new { m.CursoId, m.UsuarioId }).IsUnique();
            entity.Property(m => m.FechaRegistro).IsRequired();
            entity.Property(m => m.Estado).IsRequired();

            entity.HasOne(m => m.Curso)
                .WithMany(c => c.Matriculas)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
