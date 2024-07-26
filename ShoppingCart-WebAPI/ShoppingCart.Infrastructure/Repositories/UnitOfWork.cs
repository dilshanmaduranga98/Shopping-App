using Microsoft.EntityFrameworkCore.Storage;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShoppingDbContext _shoppingDbContext;
        private IDbContextTransaction _dbContextTransaction;
        private readonly Dictionary<Type, object> _repositories;


        // Constructor initializes the shoppingDbContext and repository dictionary.
        public UnitOfWork(ShoppingDbContext shoppingDbContext)
        {
            _shoppingDbContext = shoppingDbContext;
            _repositories = new Dictionary<Type, object>();
        }


        // Begins a new database transaction asynchronously.
        public async Task BeginTransactionAsync()
        {
            _dbContextTransaction = await _shoppingDbContext.Database.BeginTransactionAsync();
        }


        // Commits the active transaction asynchronously; rolls back on failure and disposes of the transaction object.
        public async  Task CommitAsync()
        {
            try
            {
                await _dbContextTransaction.CommitAsync();

            }catch
            {
                await _dbContextTransaction.RollbackAsync();
                throw;

            }finally
            {
                await _dbContextTransaction.DisposeAsync();
                _dbContextTransaction = null!;
            }
        }

        // Rolls back the active transaction asynchronously and disposes of the transaction object.
        public async Task RollbackAsync()
        {
            await _dbContextTransaction.RollbackAsync();
            await _dbContextTransaction.DisposeAsync();
            _dbContextTransaction = null;
        }


        // Disposes of the shoppingDbContext instance.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Indicates if the instance has been disposed.
        private bool disposed = false;


        // Dispose(bool disposing) method for releasing managed resources.
        protected virtual void Dispose(bool disposing) 
        {
            if (!this.disposed)
            {
                if(disposing) 
                {
                    _shoppingDbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        // Retrieves the base repository for type T, creating a new instance if not already present.
        public IBaseRepository<T> GetBaseRepository<T>() where T : class
        {
            if(_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as IBaseRepository<T>;
            }

            var repository = new BaseRopository<T>(_shoppingDbContext);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        // Saves all changes made in the shoppingDbContext to the database asynchronously.
        public async Task<int> SaveChangesAsync()
        {
            return await _shoppingDbContext.SaveChangesAsync();
        }
    }
}
