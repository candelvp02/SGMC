namespace SGMC.Application.Dto.System
{
    public class StatusDto
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }

    public class CreateStatusDto
    {
        public string StatusName { get; set; } = string.Empty;
    }

    public class UpdateStatusDto
    {
        public string StatusName { get; set; } = string.Empty;
    }
}