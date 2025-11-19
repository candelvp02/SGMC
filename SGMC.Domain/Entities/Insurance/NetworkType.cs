using System.ComponentModel.DataAnnotations.Schema;

namespace SGMC.Domain.Entities.Insurance
{
    [Table("NetworkType", Schema = "Insurance")]

    public partial class NetworkType
    {
        public int NetworkTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<InsuranceProvider> InsuranceProviders { get; set; } = new List<InsuranceProvider>();
    }
}