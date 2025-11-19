namespace SGMC.Application.Dto.Appointments
{
    //base record with common properties
    public record AppointmentBaseDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
    }

    // create dto inheriting from base
    public record CreateAppointmentDto : AppointmentBaseDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    //main dto inheriting from base + adding display properties
    public record AppointmentDto : AppointmentBaseDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public object? Id { get; set; }
    }

    //update dto inheriting from base + adding update-specific properties
    public record UpdateAppointmentDto
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int StatusId { get; set; }
        public int Id { get; set; }
        public string? Notes { get; set; }
    }

    //statistics dto
    public record AppointmentStatisticsDto
    {
        public int TotalAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public decimal CancellationRate { get; set; }
        public decimal ConfirmationRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    //summary dto
    public record AppointmentSummaryDto
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public int StatusId { get; set; }
    }

    //report filter dto
    public record ReportFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int? StatusId { get; set; }
        public short? SpecialtyId { get; set; }
    }

    // result dto
    public record AppointmentResultDto
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? PatientName { get; set; } = string.Empty;
        public string? DoctorName { get; set; } = string.Empty;
        public string? StatusName { get; set; } = string.Empty;
    }

    // filterdto
    public record AppointmentFilterDto
    {
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public int? StatusId { get; set; }
    }
}
