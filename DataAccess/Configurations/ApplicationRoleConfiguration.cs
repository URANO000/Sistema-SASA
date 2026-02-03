using DataAccess.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> b)
        {
            b.ToTable("AspNetRoles");
            b.Property(r => r.Estado).HasColumnName("estado").HasDefaultValue(true);
        }
    }
}
