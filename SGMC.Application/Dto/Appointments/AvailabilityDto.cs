namespace SGMC.Application.Dto.Appointments
{
    public class AvailabilityDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public int AvailabilityId { get; internal set; }
        public object? DayOfWeek { get; internal set; }
        public bool IsActive { get; internal set; }
        public object? CreatedAt { get; internal set; }
    }
    public class CreateAvailabilityDto
    {
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DayOfWeek { get; internal set; }
    }

    public class UpdateAvailabilityDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public int AvailabilityId { get; internal set; }
        public int DayOfWeek { get; internal set; }
    }
    public class AvailabilityModeDto
    {
        public short AvailabilityModeId { get; set; }
        public string AvailabilityMode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}