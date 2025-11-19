using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.Medical;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Repositories.Medical;
using SGMC.Persistence.Repositories.Users;

namespace SGMC.Infrastructure.Dependencies
{
    public static class DoctorDependency
    {
        public static IServiceCollection AddDoctorDependencies(this IServiceCollection services)
        {
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddTransient<IDoctorService, DoctorService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();

            return services;

        }
    }
}