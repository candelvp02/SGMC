using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Appointments;
using SGMC.Domain.Entities.Medical;
using SGMC.Domain.Entities.System;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Repositories.Appointments;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Appointments;


namespace SGMC.Tests.Repositories
{
    public class AppointmentRepositoryTests : IDisposable
    {
        private readonly IAppointmentRepository _repository;
        private readonly HealtSyncContext _context;

        public AppointmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"AppointmentDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new AppointmentRepository(_context);
            SeedRequiredData();
        }

        private void SeedRequiredData()
        {
            // Add Status
            if (!_context.Statuses.Any())
            {
                _context.Statuses.Add(new Status { StatusId = 1, StatusName = "Scheduled" });
                _context.SaveChanges();
            }

            // Add Person for Patient
            if (!_context.Persons.Any(p => p.PersonId == 10))
            {
                _context.Persons.Add(new Person
                {
                    PersonId = 10,
                    FirstName = "John",
                    LastName = "Doe",
                    IdentificationNumber = "ID123"
                });
                _context.SaveChanges();
            }

            // Add Patient
            if (!_context.Patients.Any(p => p.PatientId == 10))
            {
                _context.Patients.Add(new Patient
                {
                    PatientId = 10,
                    PhoneNumber = "555-0100",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                });
                _context.SaveChanges();
            }

            // Add Person for Doctor
            if (!_context.Persons.Any(p => p.PersonId == 1))
            {
                _context.Persons.Add(new Person
                {
                    PersonId = 1,
                    FirstName = "Dr",
                    LastName = "Smith",
                    IdentificationNumber = "DOC001"
                });
                _context.SaveChanges();
            }

            // Add Specialty
            if (!_context.Specialties.Any(s => s.SpecialtyId == 1))
            {
                _context.Specialties.Add(new Specialty
                {
                    SpecialtyId = 1,
                    SpecialtyName = "General",
                    IsActive = true
                });
                _context.SaveChanges();
            }

            // Add Doctor
            if (!_context.Doctors.Any(d => d.DoctorId == 1 || d.DoctorId == 5))
            {
                _context.Doctors.AddRange(
                    new Doctor
                    {
                        DoctorId = 1,
                        SpecialtyId = 1,
                        LicenseNumber = "L001",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    },
                    new Doctor
                    {
                        DoctorId = 5,
                        SpecialtyId = 1,
                        LicenseNumber = "L005",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    }
                );
                _context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetByPatientIdAsync_ReturnsCorrectAppointments()
        {
            var appt = new Appointment
            {
                PatientId = 10,
                DoctorId = 1,
                AppointmentDate = DateTime.Now,
                StatusId = 1,
                CreatedAt = DateTime.Now
            };
            await _context.Appointments.AddAsync(appt);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByPatientIdAsync(10);
            Assert.Single(result);
            Assert.Equal(10, result.First().PatientId);
        }

        [Fact]
        public async Task ExistsInTimeSlotAsync_ReturnsTrue_WhenConflictExists()
        {
            var appt = new Appointment
            {
                DoctorId = 5,
                PatientId = 10,
                AppointmentDate = new DateTime(2025, 6, 10, 10, 0, 0),
                StatusId = 1,
                CreatedAt = DateTime.Now
            };
            await _context.Appointments.AddAsync(appt);
            await _context.SaveChangesAsync();

            var exists = await _repository.ExistsInTimeSlotAsync(5, new DateTime(2025, 6, 10, 10, 0, 0));
            Assert.True(exists);
        }

        [Fact]
        public async Task AddAsync_AddsNewAppointment()
        {
            var appt = new Appointment
            {
                PatientId = 10,
                DoctorId = 1,
                AppointmentDate = DateTime.Now.AddDays(1),
                StatusId = 1,
                CreatedAt = DateTime.Now
            };
            await _repository.AddAsync(appt);

            var count = await _context.Appointments.CountAsync();
            Assert.True(count >= 1);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}