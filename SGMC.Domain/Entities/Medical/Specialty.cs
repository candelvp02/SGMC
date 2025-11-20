using SGMC.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace SGMC.Domain.Entities.Medical
{
    [Table("Specialties", Schema = "medical")]
    public partial class Specialty
    {
        public short SpecialtyId { get; set; }

        public string SpecialtyName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

        public static explicit operator Specialty(string v)
        {
            throw new NotImplementedException();
        }
    }
}