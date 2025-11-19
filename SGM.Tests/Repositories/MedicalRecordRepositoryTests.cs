using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Medical;
using SGMC.Domain.Repositories.Medical;
using SGMC.Persistence.Context;
using SGMC.Persistence.Repositories.Medical;


namespace SGMC.Tests.Repositories
{
    public class MedicalRecordRepositoryTests
    {
        private readonly IMedicalRecordRepository _repository;
        private readonly HealtSyncContext _context;

        public MedicalRecordRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HealtSyncContext>()
                .UseInMemoryDatabase($"MedicalRecordDB_{Guid.NewGuid()}")
                .Options;
            _context = new HealtSyncContext(options);
            _repository = new MedicalRecordRepository(_context);
        }

        [Fact]
        public async Task GetByPatientIdAsync_ReturnsRecords()
        {
            await _context.MedicalRecords.AddAsync(new MedicalRecord
            {
                PatientId = 1,
                DoctorId = 1,
                Diagnosis = "Flu",
                DateOfVisit = DateTime.Now,
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetByPatientIdAsync(1);
            Assert.Single(result);
            Assert.Equal("Flu", result.First().Diagnosis);
        }
    }
}