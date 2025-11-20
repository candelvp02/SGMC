using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.Users
{
    [Table("Persons", Schema = "users")]
    public partial class Person
    {
        public int PersonId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        public string IdentificationNumber { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public virtual Doctor? Doctor { get; set; }

        public virtual Employee? Employee { get; set; }

        public virtual Patient? Patient { get; set; }

        public virtual User? User { get; set; }
    //    public int UserId { get; set; }
    }
}