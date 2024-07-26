using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Application.Interfaces.IRepositories
{

    // Defines the Unit of Work interface for managing transactions and repositories
    public interface IUnitOfWork : IDisposable
    {

        // Retrieves a specific repository based on type T
        IBaseRepository<T> GetBaseRepository<T>() where T : class;


        // Begins a database transaction asynchronously
        Task BeginTransactionAsync();

        // Commits changes made during the transaction asynchronously
        Task CommitAsync();

        // Rolls back changes made during the transaction asynchronously
        Task RollbackAsync();


        // Saves all changes made in the context to the database asynchronously
        Task<int> SaveChangesAsync();
    }
}
