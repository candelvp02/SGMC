using SGMC.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.Medical
{
    [Table("AvailabilityModes", Schema = "medical")]
    public partial class AvailabilityMode
    {
        public short AvailabilityModeId { get; set; }

        public string AvailabilityMode1 { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}