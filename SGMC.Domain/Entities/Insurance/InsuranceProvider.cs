using SGMC.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.Insurance
{
    [Table("InsuranceProviders", Schema = "Insurance")]
    public partial class InsuranceProvider
    {
        public int InsuranceProviderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string? CoverageDetails { get; set; }
        public string LogoUrl { get; set; } = string.Empty;
        public bool IsPreferred { get; set; }
        public int NetworkTypeId { get; set; }
        public string CustomerSupportContact { get; set; } = string.Empty;
        public string AcceptedRegions { get; set; } = string.Empty;
        public decimal? MaxCoverageAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public virtual NetworkType? NetworkType { get; set; }
        public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}