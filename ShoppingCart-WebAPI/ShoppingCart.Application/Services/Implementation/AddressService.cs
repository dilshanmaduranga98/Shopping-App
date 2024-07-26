using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Domain.Models;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Services.Interfaces;
using ShoppingCart.Application.Interfaces.IRepositories;



namespace ShoppingCart.Application.Services.Implementation
{

    // Implementation of the IAddressService interface
    public class AddressService : IAddressService  // implements from IAddressService interface
    {
        //private readonly IBaseRepository<Address> _addressRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;  //use to access access token get from header
        private readonly ITokenServices _tokenServices; //token service interface
        private readonly ILogger<AddressService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        //constructor
        public AddressService(
            IHttpContextAccessor httpContextAccessor,
            ITokenServices tokenServices, 
            IUnitOfWork unitOfWork)
        {
            //_addressRepository = addressRepository;
            _httpContextAccessor = httpContextAccessor;
            _tokenServices = tokenServices;
            _unitOfWork = unitOfWork;
        }



        // Method for adding a new address
        public async Task<Address> AddAddress(AddressDTO addressReq)
        {

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                // Get the user's access information
                var accessUser = _httpContextAccessor.HttpContext.User;

                // Get the user ID from the token
                var user_ID = _tokenServices.GetUserIDClaim(); // this data come from ITokenServices >> TokenServices >> GetUserIDClaim


                // Create a new address object
                var newAddress = new Address
                {
                    userID = user_ID,
                    street = addressReq.street,
                    city = addressReq.city,
                    country = addressReq.country,
                    postalCode = addressReq.postalCode,
                    IsPrimary = false,
                };

                // Add the new address to the database

                //await _addressRepository.AddAsync(newAddress);
                await _unitOfWork.GetBaseRepository<Address>().AddAsync(newAddress);
                await _unitOfWork.SaveChangesAsync();

                // Log the addition of the new address
                Log.Information("Add new address => {@newAddress}", newAddress);
                await _unitOfWork.CommitAsync();
                return newAddress;
            }catch
            {
                await _unitOfWork.RollbackAsync();
                throw new("Error Occured during add an new address!");
            }
        }


        // Method for view addresses
        public async Task<List<Address>> ViewAddress()
        {
            // Get the user's access information
            var accessUser = _httpContextAccessor.HttpContext.User;

            // Get the user ID from the token
            var user_ID = _tokenServices.GetUserIDClaim(); // this data come from ITokenServices >> TokenServices >> GetUserIDClaim


            var addressesData = await _unitOfWork.GetBaseRepository<Address>().GetByIDAsync(user_ID, a => a.userID == user_ID);


            if (addressesData == null)
            {
                throw new Exception("Address data not found!");
            }


            // Log the viewed address
            Log.Information($"all addresses of  user: {user_ID} => {addressesData}");
            return addressesData;
        }


        //Method for view User Info from DB
        public async Task<User> ViewUserDetails()
        {
            // Get the user's access information
            var accessUser = _httpContextAccessor.HttpContext.User;

            // Get the user ID from the token
            var user_ID = _tokenServices.GetUserIDClaim(); // this data come from ITokenServices >> TokenServices >> GetUserIDClaim


            var UserData = await _unitOfWork.GetBaseRepository<User>().FirstOrDefaultAsyncByID(user_ID, a => a.userID == user_ID);


            if (UserData == null)
            {
                throw new Exception("User data not found!");
            }


            // Log the viewed address
            Log.Information($"User Data: {user_ID} => {UserData}");
            return UserData;
        }
    }
}
