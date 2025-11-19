using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMC.Domain.Entities.Medical;

namespace SGMC.Persistence.Configuration.Medical
{
    public partial class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
    {
        public void Configure(EntityTypeBuilder<Specialty> entity)
        {
            entity.HasKey(e => e.SpecialtyId).HasName("PK__Specialt__D768F648D93CE967");

            entity.ToTable("Specialties", "medical");

            entity.HasIndex(e => e.SpecialtyName, "UQ__Specialt__7DCA574800EB0679").IsUnique();

            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF_Specialties_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasAnnotation("Relational:DefaultConstraintName", "DF_Specialties_IsActive");
            entity.Property(e => e.SpecialtyName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF_Specialties_UpdatedAt")
                .HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Specialty> entity);
    }
}