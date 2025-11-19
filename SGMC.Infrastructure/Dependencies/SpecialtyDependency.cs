using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.Medical;
using SGMC.Persistence.Repositories.Medical;

namespace SGMC.Infrastructure.Dependencies
{
    public static class SpecialtyDependency
    {
        public static void AddSpecialtyDependencies(this IServiceCollection services)
        {
            services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
            services.AddTransient<ISpecialtyService, SpecialtyService>();
        }
    }
}