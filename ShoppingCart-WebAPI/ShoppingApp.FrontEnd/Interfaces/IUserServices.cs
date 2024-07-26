using ShoppingApp.FrontEnd.Models;

namespace ShoppingApp.FrontEnd.Interfaces
{
    public interface IUserServices
    {

        //service for register the new user
         Task<bool> RegisterUser(UserModel user);


        //service for login to user
        Task<TokenModel> GetAccessToken(LoginModel loginData);


        //service for get user info
        Task<UserModel> UserInfo();

        //service for get user address infomations
        Task<List<AddressModel>> UserAddressInfo();
    }
}
