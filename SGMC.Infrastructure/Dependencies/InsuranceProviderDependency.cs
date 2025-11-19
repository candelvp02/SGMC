using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.Insurance;
using SGMC.Persistence.Repositories.Insurance;

namespace SGMC.Infrastructure.Dependencies
{
    public static class InsuranceProviderDependency
    {
        public static IServiceCollection AddInsuranceProviderDependencies(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IInsuranceProviderRepository, InsuranceProviderRepository>();
            services.AddScoped<INetworkTypeRepository, NetworkTypeRepository>();

            // Services
            services.AddTransient<IInsuranceProviderService, InsuranceProviderService>();

            return services;
        }
    }
}
