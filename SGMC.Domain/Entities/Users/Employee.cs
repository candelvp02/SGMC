using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.Users
{
    [Table("Employees", Schema = "users")]
    public partial class Employee
    {
        public int EmployeeId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual Person? EmployeeNavigation { get; set; }
    }
}