using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Entities.Medical;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.Users
{
    [Table("Doctors", Schema = "users")]
    public partial class Doctor
    {
        public int DoctorId { get; set; }

        public short SpecialtyId { get; set; }

        public string LicenseNumber { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public int YearsOfExperience { get; set; }

        public string Education { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public decimal? ConsultationFee { get; set; }

        public string ClinicAddress { get; set; } = string.Empty;

        public short? AvailabilityModeId { get; set; }

        public DateOnly LicenseExpirationDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public virtual AvailabilityMode? AvailabilityMode { get; set; }

        public virtual ICollection<DoctorAvailability> DoctorAvailabilities { get; set; } = new List<DoctorAvailability>();

        public virtual Person? DoctorNavigation { get; set; }

        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        public virtual Specialty? Specialty { get; set; }
        public bool Exitoso { get; set; }
        public string? Mensaje { get; set; }
        public string Datos { get; set; } = string.Empty;

        public static explicit operator Doctor(string v)
        {
            throw new NotImplementedException();
        }
    }
}