using System.Security.Claims;

namespace ShoppingCart.Application.Interfaces
{

    // Interface for token-related services
    public interface ITokenServices
    {
        // Method to extract the UserID claim from a collection of claims
        string GetUserIDClaim();
    }
}
