using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMC.Domain.Entities.Users;

namespace SGMC.Persistence.Configuration.Users
{
    public partial class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> entity)
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EDFCF31D9E9");

            entity.ToTable("Doctors", "users");

            entity.Property(e => e.DoctorId)
                .ValueGeneratedNever()
                .HasColumnName("DoctorID");
            entity.Property(e => e.ClinicAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ConsultationFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasAnnotation("Relational:DefaultConstraintName", "DF_Doctors_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.Education).IsRequired();
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasAnnotation("Relational:DefaultConstraintName", "DF_Doctors_IsActive");
            entity.Property(e => e.LicenseNumber)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.AvailabilityMode).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.AvailabilityModeId)
                .HasConstraintName("FK_Doctors_AvailabilityMode");

            entity.HasOne(d => d.DoctorNavigation).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_Person");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_Speciality");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Doctor> entity);
    }
}
