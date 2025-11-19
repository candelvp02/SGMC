namespace SGMC.Application.Dto.Insurance
{
    public class NetworkTypeDto
    {
        public int NetworkTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
    public class CreateNetworkTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}