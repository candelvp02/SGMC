using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMC.Domain.Entities.System;

namespace SGMC.Persistence.Configuration.System
{
    public partial class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entity)
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A0AF7665C");

            entity.ToTable("Roles", "system");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61603EE5AFD9").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF_Roles_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasAnnotation("Relational:DefaultConstraintName", "DF_Roles_IsActive");
            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF_Roles_UpdatedAt")
                .HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Role> entity);
    }
}