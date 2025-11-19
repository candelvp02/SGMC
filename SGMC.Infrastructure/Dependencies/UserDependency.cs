using Microsoft.Extensions.DependencyInjection;
using SGMC.Application.Interfaces.Service;
using SGMC.Application.Services;
using SGMC.Domain.Repositories.System;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Repositories.System;
using SGMC.Persistence.Repositories.Users;

namespace SGMC.Infrastructure.Dependencies
{
    public static class UserDependency
    {
        public static void AddUserDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IRoleRepository, RoleRepository>();
        }
    }
}