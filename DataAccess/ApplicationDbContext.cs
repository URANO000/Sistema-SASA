using DataAccess.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        //Aquí van los DbSet para las entidades
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tiquete> Tiquetes { get; set; }
        //Luego agrego lo de Identity (mafeh)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { //esto es para configurar las entidades y relaciones (por si lo ocupan o sino hagan caso omiso)
            base.OnModelCreating(modelBuilder);
        } 
    }


}
