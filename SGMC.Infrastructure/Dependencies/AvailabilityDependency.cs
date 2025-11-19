using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Persistence.Repositories.Appointments;

namespace SGMC.Infrastructure.Dependencies
{
    public static class AvailabilityDependency
    {
        public static void AddAvailabilityDependencies(this IServiceCollection services)
        {
            services.AddScoped<IDoctorAvailabilityRepository, DoctorAvailabilityRepository>();
            services.AddTransient<IAvailabilityService, AvailabilityService>();
        }
    }
}
