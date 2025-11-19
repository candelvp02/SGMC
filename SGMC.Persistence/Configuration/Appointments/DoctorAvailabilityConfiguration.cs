using SGMC.Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SGMC.Persistence.Configuration.Appointments
{
    public partial class DoctorAvailabilityConfiguration : IEntityTypeConfiguration<DoctorAvailability>
    {
        public void Configure(EntityTypeBuilder<DoctorAvailability> entity)
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__DoctorAv__DA397991EEC28676");

            entity.ToTable("DoctorAvailability", "appointments");

            entity.Property(e => e.AvailabilityId).HasColumnName("AvailabilityID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorAvailabilities)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__DoctorAva__Docto__5535A963");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<DoctorAvailability> entity);
    }
}