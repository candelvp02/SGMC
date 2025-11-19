using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMC.Domain.Entities.Insurance;

namespace SGMC.Persistence.Configuration.Insurance
{
    public partial class InsuranceProviderConfiguration : IEntityTypeConfiguration<InsuranceProvider>
    {
        public void Configure(EntityTypeBuilder<InsuranceProvider> entity)
        {
            entity.ToTable("InsuranceProviders", "Insurance");

            entity.Property(e => e.InsuranceProviderId).HasColumnName("InsuranceProviderID");
            entity.Property(e => e.AcceptedRegions)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF__Insurance__Creat__2CF2ADDF")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerSupportContact)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasAnnotation("Relational:DefaultConstraintName", "DF__Insurance__IsAct__2DE6D218");
            entity.Property(e => e.IsPreferred).HasAnnotation("Relational:DefaultConstraintName", "DF__Insurance__IsPre__2BFE89A6");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MaxCoverageAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.NetworkType).WithMany(p => p.InsuranceProviders)
                .HasForeignKey(d => d.NetworkTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsuranceProviders_NetworkType");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<InsuranceProvider> entity);
    }
}