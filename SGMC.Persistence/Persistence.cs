using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Domain.Repositories.Insurance;
using SGMC.Domain.Repositories.Medical;
using SGMC.Domain.Repositories.System;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Common;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Appointments;
using SGMC.Persistence.Repositories.Insurance;
using SGMC.Persistence.Repositories.Medical;
using SGMC.Persistence.Repositories.System;
using SGMC.Persistence.Repositories.Users;


namespace SGMC.Persistence
{
    public static class Persistence
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration cfg)
        {
            // DbContext
            services.AddDbContext<HealtSyncContext>(options =>
                options.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

            // StoredProcedure Executor
            services.AddScoped<StoredProcedureExecutor>();

            // Appointments Repositories
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IDoctorAvailabilityRepository, DoctorAvailabilityRepository>();

            // Insurance Repositories
            services.AddScoped<IInsuranceProviderRepository, InsuranceProviderRepository>();
            services.AddScoped<INetworkTypeRepository, NetworkTypeRepository>();

            // Medical Repositories
            services.AddScoped<IAvailabilityModeRepository, AvailabilityModeRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();

            // System Repositories
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IStatusRepository, StatusRepository>();

            // Users Repositories
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
