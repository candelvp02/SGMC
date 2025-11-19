using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.Medical;
using SGMC.Persistence.Repositories.Medical;

namespace SGMC.Infrastructure.Dependencies
{
    public static class MedicalRecordDependency
    {
        public static void AddMedicalRecordDependencies(this IServiceCollection services)
        {
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddTransient<IMedicalRecordService, MedicalRecordService>();
        }
    }
}
