using DataAccess.Modelos.Entidades;
using DataAccess.Modelos.Enums;
﻿using DataAccess.Identity;
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
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tiquete> Tiquetes { get; set; }
        public DbSet<Estatus> Estatuses { get; set; }
        public DbSet<Prioridad> Prioridades { get; set; } 
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cola> Colas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seeder de enum
            modelBuilder.Entity<Estatus>()
                .HasData(
                    new Estatus { IdEstatus = (int)TiqueteEstatus.Abierto, NombreEstatus = "Abierto" },
                    new Estatus { IdEstatus = (int)TiqueteEstatus.EnProgreso, NombreEstatus = "En Progreso"},
                    new Estatus { IdEstatus = (int)TiqueteEstatus.Cancelado, NombreEstatus = "Cancelado"},
                    new Estatus { IdEstatus = (int)TiqueteEstatus.Resuelto, NombreEstatus = "Resuelto"},
                    new Estatus { IdEstatus = (int)TiqueteEstatus.Cerrado, NombreEstatus = "Cerrado"}
                );

            //Tablas "intermedias" de Identity (como en el script)
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");

            //Aplica todas las configuraciones (User/Role y luego las que agreguen para Tiquete, etc.)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        } 

    }
}
