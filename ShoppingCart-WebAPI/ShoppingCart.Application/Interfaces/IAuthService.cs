

using ShoppingCart.Application.DTOs;

namespace ShoppingCart.Application.Interfaces
{

    // Interface for authentication services
    public interface IAuthService
    {
        // Method for user signup using Auth0
        Task<string> userSignup(UserDTO userReq);


        // Method for user login using Auth0
        Task<AccessTokenDTO> userLogin(string email, string password);
     

        //retrive acces token using refresh token
        Task<AccessTokenDTO> GetAccessTokenFromRefreshToken(string token);
    }
}
