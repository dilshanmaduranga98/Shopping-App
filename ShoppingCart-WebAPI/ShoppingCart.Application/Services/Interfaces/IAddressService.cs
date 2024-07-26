using ShoppingCart.Application.DTOs; // Importing DTOs for Address
using ShoppingCart.Domain.Models; // Importing Models for Address


namespace ShoppingCart.Application.Services.Interfaces
{
    //interface for all address services
    public interface IAddressService
    {
        // Method to add a new address
        Task<Address> AddAddress(AddressDTO addressReq);


        //Method to view address
        Task<List<Address>> ViewAddress();


        //Method to view User info from DB
        Task<User> ViewUserDetails();
    }
}
