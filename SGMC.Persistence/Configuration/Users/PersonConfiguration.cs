using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGMC.Domain.Entities.Users;

namespace SGMC.Persistence.Configuration.Users
{
    public partial class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> entity)
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Persons__AA2FFB85C91EBDF7");

            entity.ToTable("Persons", "users");

            entity.Property(e => e.PersonId)
                .ValueGeneratedNever()
                .HasColumnName("PersonID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IdentificationNumber)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(40)
                .IsUnicode(false);

            entity.HasOne(p => p.User)
                  .WithOne(u => u.UserNavigation)
                  .HasForeignKey<Person>(p => p.PersonId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Persons_Users");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Person> entity);
    }
}