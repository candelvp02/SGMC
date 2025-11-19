using SGMC.Domain.Entities.System;
using SGMC.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace SGMC.Domain.Entities.Appointments
{
    [Table("Appointments", Schema = "appointments")]
    public partial class Appointment
    {
        public int AppointmentId { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public int StatusId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual Doctor? Doctor { get; set; }
        public virtual Status? Status { get; set; }
    }
}