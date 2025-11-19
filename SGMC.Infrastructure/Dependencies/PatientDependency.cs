using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.Insurance;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Repositories.Insurance;
using SGMC.Persistence.Repositories.Users;

namespace SGMC.Infrastructure.Dependencies
{
    public static class PatientDependency
    {
        public static void AddPatientDependencies(this IServiceCollection services)
        {
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddTransient<IPatientService, PatientService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IInsuranceProviderRepository, InsuranceProviderRepository>();
        }
    }
}
