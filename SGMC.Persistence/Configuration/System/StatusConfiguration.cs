using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMC.Domain.Entities.System;

namespace SGMC.Persistence.Configuration.System
{
    public partial class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> entity)
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Status__C8EE2043759308C1");

            entity.ToTable("Status", "system");

            entity.HasIndex(e => e.StatusName, "UQ__Status__05E7698AD7258224").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Status> entity);
    }
}
