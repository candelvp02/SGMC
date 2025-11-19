using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMC.Domain.Entities.Users;

namespace SGMC.Persistence.Configuration.Users
{
    public partial class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> entity)
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC346B158B7FB");
            entity.ToTable("Patients", "users");

            entity.Property(e => e.PatientId)
                .ValueGeneratedNever()
                .HasColumnName("PatientID");
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Allergies).IsRequired();
            entity.Property(e => e.BloodType)
                .IsRequired()
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF_Patients_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.EmergencyContactName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.EmergencyContactPhone)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.InsuranceProviderId).HasColumnName("InsuranceProviderID");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasAnnotation("Relational:DefaultConstraintName", "DF_Patients_IsActive");
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.InsuranceProvider).WithMany(p => p.Patients)
                .HasForeignKey(d => d.InsuranceProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_InsuranceProvider");

            entity.HasOne(d => d.PatientNavigation).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_Person");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Patient> entity);
    }
}