using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Persistence.Repositories.Appointments;

namespace SGMC.Infrastructure.Dependencies
{
    public static class AppointmentDependency
    {
        public static void AddAppointmentDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddTransient<IAppointmentService, AppointmentService>();

            services.AddScoped<IAppointmentService, AppointmentService>();

        }
    }
}