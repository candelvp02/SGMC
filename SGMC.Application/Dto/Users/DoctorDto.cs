using SGMC.Application.Dto.Base;

namespace SGMC.Application.Dto.Users
{
    public class DoctorDto : PersonBaseDto
    {
        public int DoctorId { get; set; }
        public string Email { get; set; } = string.Empty;
        public short SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string Education { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? ClinicAddress { get; set; }
        public string? AvailabilityMode { get; set; }
        public DateOnly LicenseExpirationDate { get; set; }
        public bool IsActive { get; set; }
    }
    public class RegisterDoctorDto : RegisterPersonBaseDto
    {
        public short SpecialtyId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string Education { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? ClinicAddress { get; set; }
        public DateOnly LicenseExpirationDate { get; set; }
    }
    public class UpdateDoctorDto
    {
        public int DoctorId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string Education { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? ClinicAddress { get; set; }
        public short? AvailabilityMode { get; set; }
        public DateOnly LicenseExpirationDate { get; set; }
    }
}