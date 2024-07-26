using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using ShoppingCart.Application.Interfaces;
using System.Security.Claims;


namespace ShoppingCart.Infrastructure.Services
{
    public class TokenService:ITokenServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        // Constructor to initialize TokenService with required dependencies.
        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        // Method to retrieve user ID claim from the provided claims.
        public string GetUserIDClaim()
        {

            var access = _httpContextAccessor.HttpContext.User;
            var claim = access.Claims;

            // Retrieving the user ID claim.
            var userIdClaim = claim.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
           

            if (userIdClaim != null)
            {
                // Splitting the user ID claim value.
                string[] parts = userIdClaim.Value.Split('|');


                if (parts.Length == 2)
                {
                    Log.Information("user ID => {@partsOne}", parts[1]);

                    // Returning the user ID.
                    return parts[1];
                }
                else
                {
                    Log.Error("length not equal to 2!");
                    throw new InvalidOperationException("The user ID claim is not in the expected format.");
                }
            }
            else
            {
                Log.Error("Unauthorized, user didn't give access token!");
                throw new InvalidOperationException("The user ID claim was not found.");
            }

        }



    }
}
