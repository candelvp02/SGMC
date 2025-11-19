using System.Linq.Expressions;

namespace SGMC.Domain
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(); //obtener todas las entidades
        Task<T?> GetByIdAsync(int id); //obtener una entidad por su id
        Task<T> AddAsync(T entity); //agregar una nueva entidad
        Task UpdateAsync(T entity); //actualizar una entidad existente
        Task DeleteAsync(int id); //eliminar una entidad por su id
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate); //buscar entidades que cumplan con condicion where
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate); //verificar si existe 
    }
}