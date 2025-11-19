using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;

namespace SGMC.Infrastructure.Dependencies
{
    public static class ReportDependency
    {
        public static void AddReportDependencies(this IServiceCollection services)
        {
            services.AddTransient<IReportService, ReportService>();
        }
    }
}