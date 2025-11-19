using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMC.Domain.Entities.Insurance;

namespace SGMC.Persistence.Configuration.Insurance
{
    public partial class NetworkTypeConfiguration : IEntityTypeConfiguration<NetworkType>
    {
        public void Configure(EntityTypeBuilder<NetworkType> entity)
        {
            entity.HasKey(e => e.NetworkTypeId).HasName("PK__NetworkT__C09029EE538D704E");

            entity.ToTable("NetworkType", "Insurance");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF__NetworkTy__Creat__29221CFB")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasAnnotation("Relational:DefaultConstraintName", "DF__NetworkTy__IsAct__2A164134");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<NetworkType> entity);
    }
}