using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Infrastructure.Data;
using System.Linq.Expressions;


namespace ShoppingCart.Infrastructure.Repositories
{
    public class BaseRopository<T> : IBaseRepository<T> where T : class
    {
        private readonly ShoppingDbContext _shoppingDbContext;
        private readonly DbSet<T> _dbSet;

        public BaseRopository(ShoppingDbContext shoppingDbContext)
        {
            _shoppingDbContext = shoppingDbContext;
            _dbSet = shoppingDbContext.Set<T>();
        }

        // Retrieves all entities of type T from the database.
        public async Task<List<T>> GetAll()
        {
            var products = await _dbSet.ToListAsync();
            return products;
        }

        // Retrieves a single entity of type T by its ID from the database.
        public async Task<T> GetById(int ID)
        {
            var productData = await _dbSet.FindAsync(ID);
            return productData;
        }


        // Retrieves multiple entities of type T from the database based on a predicate.
        public async Task<List<T>> GetByIDAsync(int ID, Expression<Func<T, bool>> predict)
        {
            var result = await _dbSet.Where(predict).ToListAsync();
            return result;
        }

        // Retrieves multiple entities of type T from the database based on a string ID and a predicate.
        public async Task<List<T>> GetByIDAsync(string ID, Expression<Func<T, bool>> predict)
        {
            var result = await _dbSet.Where(predict).ToListAsync();
            return result;
        }

        // Retrieves the first or default entity of type T from the database based on a string ID and a predicate.
        public async Task<T> FirstOrDefaultAsyncByID(string ID, Expression<Func<T, bool>> predict)
        {
            var result = await _dbSet.FirstOrDefaultAsync(predict);
            return result;
        }

        // Retrieves the first or default entity of type T from the database based on an int ID and a predicate.
        public async Task<T> FirstOrDefaultAsyncByID(int ID, Expression<Func<T, bool>> predict)
        {
            var result = await _dbSet.FirstOrDefaultAsync(predict);
            return result;
        }

        // Adds a new entity of type T to the database asynchronously.
        public async Task<bool> AddAsync(T entity)
        {
            var result =  await _dbSet.AddAsync(entity);
            
            return true;// Indicating success; actual error handling should be implemented.

        }

        // Returns IQueryable<T> based on an int ID and a predicate.
        public IQueryable<T> FindByID(string ID, Expression<Func<T, bool>> predict)
        {
            var result = _dbSet.Where(predict);
            return result;
        }

        // Returns IQueryable<T> based on a string ID and a predicate.
        public IQueryable<T> FindByID(int ID, Expression<Func<T, bool>> predict)
        {
            var result = _dbSet.Where(predict);
            return result;
        }
        


        public IQueryable<T> FindByID(int IID, string SID, Expression<Func<T, bool>> predict)
        {
            var result = _dbSet.Where(predict);
            return result;
        }


        // Updates an entity of type T in the database. 
        public async Task<T> Update(T entity) 
        {
            _dbSet.Attach(entity);
            _shoppingDbContext.Entry(entity).State = EntityState.Modified;
            await SaveChanges();
            return entity;

        }

        // Updates a specific column of an entity of type T in the database based on its ID and column name.
        public async Task UpdateColumn(int id, string columnName,int newValue)
        {
            var row = await _dbSet.FindAsync(id);

            if (row != null)
            {
                var propertyInfo = row.GetType().GetProperty(columnName);
                if(propertyInfo != null) 
                {
                    propertyInfo.SetValue(row, newValue);
                    await SaveChanges();
                }
                
            }

        }

        // Removes an entity of type T from the database.
        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveChanges();
        }

        // Removes a range of entities of type T from the database.
        public async Task RemoveRangeAsync(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
            await SaveChanges();
        }

        // Saves all changes made in this context to the database.
        public async Task SaveChanges()
        {
            await _shoppingDbContext.SaveChangesAsync();
        }
    }
}
