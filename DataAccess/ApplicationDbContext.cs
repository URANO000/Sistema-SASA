using Microsoft.EntityFrameworkCore;
using DataAccess.Modelos.Entidades;

namespace DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        //Aquí van los DbSet para las entidades
        //Luego agrego lo de Identity (mafeh)
        public DbSet<Notificacion> Notificaciones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { //esto es para configurar las entidades y relaciones (por si lo ocupan o sino hagan caso omiso)
            base.OnModelCreating(modelBuilder);
        } 
    }


}
