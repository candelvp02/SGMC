using SGMC.Application.Dto.Appointments;
using SGMC.Web.Models.Appointment;

namespace SGMC.Web.Models.Report
{
    public class ReportViewModel
    {
        // Los filtros que el usuario envía
        public ReportFilterDto Filter { get; set; } = new ReportFilterDto();

        // Las estadísticas que el servicio devuelve
        public AppointmentStatisticsDto? Statistics { get; set; }

        // Listas para los dropdowns del formulario de filtro
        public List<PatientSelectViewModel>? Patients { get; set; }
        public List<DoctorSelectViewModel>? Doctors { get; set; }
        public List<StatusSelectViewModel>? Statuses { get; set; }

        // Lista para mostrar los resultados de la cita en la tabla
        public List<AppointmentResultViewModel> FilteredAppointments { get; set; } = new List<AppointmentResultViewModel>();
    }

    // Define los datos que queremos mostrar en cada fila de la tabla de resultados
    public class AppointmentResultViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? StatusName { get; set; }
        public string? StatusColor { get; set; }
    }
}