using Serilog;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Application.Services.Interfaces;
using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Application.Services.Implementation
{
    public class UpdateTimeStampService : IUpdateTimeStampService
    {
        private readonly IUnitOfWork _unitOfWork; // Unit of Work for database operations

        public UpdateTimeStampService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; // Injected dependency for unit of work
        }

        // Method to update cart items' update date for a specific user
        public async Task<string> updateDateTime(string userId)
        {
            // Retrieve cart items for the specified user
            var cartUser = await _unitOfWork.GetBaseRepository<UserCart>().GetByIDAsync(userId, a => a.userID == userId);

            // Update each cart item's update date to the current date/time
            foreach (var item in cartUser)
            {
                item.cartUpdateDate = DateTime.Now;
            }


            // Log information about the cart update
            Log.Information("cart updated!");


            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();


            // Return a message indicating successful update
            return ("update time!");
        }
    }
}
