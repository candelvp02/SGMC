using SGMC.Domain.Entities.Appointments;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.System
{
    [Table("Status", Schema = "system")]
    public partial class Status
    {
        public int StatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
