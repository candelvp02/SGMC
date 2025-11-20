using SGMC.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace SGMC.Domain.Entities.Medical
{
    [Table("MedicalRecords", Schema = "medical")]

    public partial class MedicalRecord
    {
        public int RecordId { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public string Diagnosis { get; set; } = string.Empty;

        public string Treatment { get; set; } = string.Empty;

        public DateTime DateOfVisit { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual Doctor? Doctor { get; set; }

        public virtual Patient? Patient { get; set; }
        public int MedicalRecordId { get; set; }
        public int Id { get; set; }
    }
}
