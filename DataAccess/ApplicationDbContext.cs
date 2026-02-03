using DataAccess.Identity;
using DataAccess.Modelos.Entidades;
using DataAccess.Modelos.Entidades.Integracion;
using DataAccess.Modelos.Entidades.Inventario;
using DataAccess.Modelos.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        //Aquí van los DbSet para las entidades
        public DbSet<Tiquete> Tiquetes { get; set; }
        public DbSet<Estatus> Estatuses { get; set; }
        public DbSet<Prioridad> Prioridades { get; set; } 
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cola> Colas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<ActivoInventario> ActivoInventario { get; set; }
        public DbSet<TipoActivoInventario> TipoActivoInventario { get; set; }
        public DbSet<EstadoActivoInventario> EstadoActivoInventario { get; set; }
        public DbSet<TipoLicenciaInventario> TipoLicenciaInventario { get; set; }
        public DbSet<IntegracionHistorial> IntegracionHistorial { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Estatus>()
            .Property(e => e.IdEstatus)
            .ValueGeneratedNever();

            //Seeder de enum
            modelBuilder.Entity<Estatus>()
                .HasData(
                    new Estatus { IdEstatus = (int)TiqueteEstatus.Abierto, NombreEstatus = "Abierto" },
                    new Estatus { IdEstatus = (int)TiqueteEstatus.EnProgreso, NombreEstatus = "En Progreso"},
                    new Estatus { IdEstatus = (int)TiqueteEstatus.Cancelado, NombreEstatus = "Cancelado"},
                    new Estatus { IdEstatus = (int)TiqueteEstatus.Resuelto, NombreEstatus = "Resuelto"},
                    new Estatus { IdEstatus = (int)TiqueteEstatus.Cerrado, NombreEstatus = "Cerrado"}
                );

            //Modelado en tiquetes

            modelBuilder.Entity<Tiquete>(entity =>
            {
                entity.HasOne(t => t.Asignee)
                    .WithMany(u => u.TiquetesAsignados)
                    .HasForeignKey(t => t.IdAsignee)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ReportedBy)
                    .WithMany(u => u.TiquetesReportados)
                    .HasForeignKey(t => t.IdReportedBy)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            //Tablas "intermedias" de Identity (como en el script)
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");

            //Aplica todas las configuraciones (User/Role y luego las que agreguen para Tiquete, etc.)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<ActivoInventario>(entity =>
            {
                entity.ToTable("ActivoInventario");
                entity.HasKey(x => x.IdActivo);

                entity.Property(x => x.NumeroActivo).HasMaxLength(40).IsRequired();
                entity.HasIndex(x => x.NumeroActivo).IsUnique();

                entity.HasOne(x => x.TipoActivo)
                    .WithMany()
                    .HasForeignKey(x => x.IdTipoActivo);

                entity.HasOne(x => x.EstadoActivo)
                    .WithMany()
                    .HasForeignKey(x => x.IdEstadoActivo);

                entity.HasOne(x => x.TipoLicencia)
                    .WithMany()
                    .HasForeignKey(x => x.IdTipoLicencia);
            });

            modelBuilder.Entity<TipoActivoInventario>(entity =>
            {
                entity.ToTable("TipoActivoInventario");
                entity.HasKey(x => x.IdTipoActivo);
                entity.Property(x => x.Nombre).HasMaxLength(80).IsRequired();
            });

            modelBuilder.Entity<EstadoActivoInventario>(entity =>
            {
                entity.ToTable("EstadoActivoInventario");
                entity.HasKey(x => x.IdEstadoActivo);
                entity.Property(x => x.Nombre).HasMaxLength(40).IsRequired();
            });

            modelBuilder.Entity<TipoLicenciaInventario>(entity =>
            {
                entity.ToTable("TipoLicenciaInventario");
                entity.HasKey(x => x.IdTipoLicencia);
                entity.Property(x => x.Nombre).HasMaxLength(60).IsRequired();
            });
        }

    }
}
