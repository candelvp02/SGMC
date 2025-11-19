using SGMC.Domain.Entities.System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.Users
{
    [Table("Users", Schema = "users")]
    public partial class User
    {
        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public int? RoleId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public virtual Role? Role { get; set; }

        public virtual Person? UserNavigation { get; set; }
        public bool Exitoso { get; set; }
        public object? Datos { get; set; }
        public string? Mensaje { get; set; }
    }
}