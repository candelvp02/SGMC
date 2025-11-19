using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMC.Domain.Entities.Medical;

namespace SGMC.Persistence.Configuration.Medical
{
    public partial class AvailabilityModeConfiguration : IEntityTypeConfiguration<AvailabilityMode>
    {
        public void Configure(EntityTypeBuilder<AvailabilityMode> entity)
        {
            entity.HasKey(e => e.AvailabilityModeId).HasName("PK__Availabi__A1FC32EB96F7E6BB");

            entity.ToTable("AvailabilityModes", "medical");

            entity.HasIndex(e => e.AvailabilityMode1, "UQ__Availabi__8598C8508EE645A1").IsUnique();

            entity.Property(e => e.AvailabilityModeId).HasColumnName("AvailabilityModeID");
            entity.Property(e => e.AvailabilityMode1)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AvailabilityMode");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF_AvailabilityModes_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasAnnotation("Relational:DefaultConstraintName", "DF_AvailabilityModes_IsActive");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF_AvailabilityModes_UpdatedAt")
                .HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<AvailabilityMode> entity);
    }
}