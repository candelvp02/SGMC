using SGMC.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.System
{
    [Table("Roles", Schema = "system")]
    public partial class Role
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}