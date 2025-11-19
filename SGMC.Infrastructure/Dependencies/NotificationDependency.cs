using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.System;
using SGMC.Infrastructure.Services;
using SGMC.Persistence.Repositories.System;

namespace SGMC.Infrastructure.Dependencies
{
    public static class NotificationDependency
    {
        public static void AddNotificationDependencies(this IServiceCollection services)
        {
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddScoped<INotificationService, MockNotificationService>();

        }
    }
}