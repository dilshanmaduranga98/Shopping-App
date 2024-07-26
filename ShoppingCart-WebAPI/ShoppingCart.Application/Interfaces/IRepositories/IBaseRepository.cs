

using System.Linq.Expressions;

namespace ShoppingCart.Application.Interfaces.IRepositories
{
    // Generic repository interface for CRUD operations
    public interface IBaseRepository<T> where T : class
    {
        // Retrieves all entities of type T
        Task<List<T>> GetAll();



        // Retrieves an entity of type T by its integer ID
        Task<T> GetById(int ID);



        // Retrieves multiple entities of type T based on an integer ID and a predicate
        Task<List<T>> GetByIDAsync(int ID, Expression<Func<T, bool>> predict);



        // Retrieves multiple entities of type T based on a string ID and a predicate
        Task<List<T>> GetByIDAsync(string ID, Expression<Func<T, bool>> predict);



        // Adds a new entity of type T to the database asynchronously
        Task<bool> AddAsync(T entity);



        // Retrieves the first entity of type T based on a string ID and a predicate
        Task<T> FirstOrDefaultAsyncByID(string ID, Expression<Func<T, bool>> predict);



        // Retrieves the first entity of type T based on an integer ID and a predicate
        Task<T> FirstOrDefaultAsyncByID(int ID, Expression<Func<T, bool>> predict);


        // Returns an IQueryable of entities of type T based on a string ID and a predicate
        IQueryable<T> FindByID(string ID, Expression<Func<T, bool>> predict);


        // Returns an IQueryable of entities of type T based on an integer ID and a predicate
        IQueryable<T> FindByID(int ID, Expression<Func<T, bool>> predict);


        // Returns an IQueryable of entities of type T based on an integer ID, a string ID, and a predicate
        IQueryable<T> FindByID(int IID, string SID, Expression<Func<T, bool>> predict);



        // Updates an existing entity of type T in the database asynchronously
        Task<T> Update(T entity);


        // Updates a specific column of an entity of type T in the database asynchronously
        Task UpdateColumn(int ID, string columnName, int newValue);



        // Removes a specific entity of type T from the database asynchronously
        Task RemoveAsync(T entity);


        // Removes a range of entities of type T from the database asynchronously
        Task RemoveRangeAsync(IEnumerable<T> entity);


        // Saves changes made in the database context asynchronously
        Task SaveChanges();
    }
}
