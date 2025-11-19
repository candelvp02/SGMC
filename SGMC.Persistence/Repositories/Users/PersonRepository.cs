using Microsoft.EntityFrameworkCore;
using SGMC.Domain.Entities.Users;
using SGMC.Domain.Repositories.Users;
using SGMC.Persistence.Base;
using SGMC.Persistence.Context;

namespace SGMC.Persistence.Repositories.Users
{
    public class PersonRepository : BaseRepository<Person>, IPersonRepository
    {
        public PersonRepository(HealtSyncContext context) : base(context) { }
        public override async Task<Person> AddAsync(Person person)
        {
            await _dbSet.AddAsync(person);
            await _context.SaveChangesAsync();
            return person;
        }
        async Task IPersonRepository.AddAsync(object person)
        {
            if (person is null) throw new ArgumentNullException(nameof(person));
            if (person is not Person p)
                throw new ArgumentException("Tipo inválido para AddAsync. Se esperaba Person.", nameof(person));

            await AddAsync(p);
        }

        public async Task<Person?> GetByIdentificationNumberAsync(string identificationNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.IdentificationNumber == identificationNumber);
        }

        public async Task<bool> ExistsByIdentificationNumberAsync(string identificationNumber)
        {
            return await _dbSet.AnyAsync(p => p.IdentificationNumber == identificationNumber);
        }

        Task IPersonRepository.DeleteAsync(int personId)
        {
            return DeleteAsync(personId);
        }

        public async Task<bool> ExistsAsync(int personId)
        {
            return await _dbSet.AnyAsync(p => p.PersonId == personId);
        }
    }
}