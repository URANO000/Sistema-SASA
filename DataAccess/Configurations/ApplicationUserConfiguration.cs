using DataAccess.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> b)
        {
            b.ToTable("AspNetUsers");

            b.Property(x => x.PrimerNombre).HasColumnName("primerNombre").HasMaxLength(50);
            b.Property(x => x.SegundoNombre).HasColumnName("segundoNombre").HasMaxLength(50);
            b.Property(x => x.PrimerApellido).HasColumnName("primerApellido").HasMaxLength(50);
            b.Property(x => x.SegundoApellido).HasColumnName("segundoApellido").HasMaxLength(50);

            b.Property(x => x.NoEmpleado).HasColumnName("noEmpleado").HasMaxLength(15);
            b.Property(x => x.Cedula).HasColumnName("cedula").HasMaxLength(20);
            b.Property(x => x.FechaNacimiento).HasColumnName("fechaNacimiento");

            // En el script: hijos BIT NOT NULL
            b.Property(x => x.Hijos).HasColumnName("hijos").HasDefaultValue(false);

            b.Property(x => x.CantidadHijos).HasColumnName("cantidadHijos");

            //En el script: departamento NVARCHAR(100)
            b.Property(x => x.Departamento).HasColumnName("departamento").HasMaxLength(100);

            //En el script: puesto NVARCHAR(100)
            b.Property(x => x.Puesto).HasColumnName("puesto").HasMaxLength(100);

            // En el script: correoEmpresa NVARCHAR(450) NULL + índice único filtrado
            b.Property(x => x.CorreoEmpresa).HasColumnName("correoEmpresa").HasMaxLength(450);

            // En el script: correoPersonal NVARCHAR(MAX)
            b.Property(x => x.CorreoPersonal).HasColumnName("correoPersonal");

            // En el script: estado BIT NOT NULL
            b.Property(x => x.Estado).HasColumnName("estado").HasDefaultValue(true);


            b.Property(x => x.SessionId).HasColumnName("sessionId").HasMaxLength(100);
            b.Property(x => x.LastActivityUtc).HasColumnName("lastActivityUtc");

            // Índices únicos filtrados (como el script)
            b.HasIndex(x => x.Cedula).IsUnique().HasFilter("[cedula] IS NOT NULL");
            b.HasIndex(x => x.CorreoEmpresa).IsUnique().HasFilter("[correoEmpresa] IS NOT NULL");
        }
    }
}
