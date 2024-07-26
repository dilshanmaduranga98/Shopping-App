using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Infrastructure.Data;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using ShoppingCart.Infrastructure.Services;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Models;
using ShoppingCart.Domain.Helper;
using Serilog;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using ShoppingCart.Application.Services.Interfaces;


//all controllers regarding User management
namespace ShoppingCart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usersController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly ILogger<usersController> _logger;
        private readonly IAddressService _addressService;



        // Constructor injection for dependencies.
        public usersController(IAuthService authService, ILogger<usersController> logger, IAddressService addressService)
        {

            _authService = authService;
            _logger = logger;
            _addressService = addressService;
        }


        // UserSignup endpoint to handle user signup.
        /// <summary>
        /// Handles user signup.
        /// </summary>
        /// <param name="userReq">The user data provided in the request body. password must contain A-Z,a-z, any symbole from this(@,_,-),a number and should be at least 8 digits.</param>
        /// <returns>An IActionResult representing the result of the signup process.</returns>
        //user signup
        [HttpPost("signup")]
        public async Task<IActionResult> UserSignup([FromBody] UserDTO userReq)
        {


            try
            {
                // Get userId from auth service
                var userID = await _authService.userSignup(userReq);

                return Ok(userID);


            }
            catch (Exception ex)
            {
                Log.Error("Error Occured while try to create new user!");
                return StatusCode(500, $"An error occurred: {ex.Message}");
                throw new Exception("Error Occured while try to create new user!");
            }

        }




        // UserLogin endpoint to handle user signin. 
        /// <summary>
        /// Handles user signin.
        /// </summary>
        /// <param name="userReq">The signin data provided in the request body.</param>
        /// <returns>An IActionResult representing the result of the signin process.</returns>
        [HttpPost("signin")]
        public async Task<IActionResult> UserLogin([FromBody] loginDTO userReq)
        {
            try
            {
                var Email = userReq.email;
                var Password = userReq.password;

                //call the userLogin service with given email and password
                var token = await _authService.userLogin(Email, Password);


                if (token != null)
                {
                    Log.Information($"User loging succesfull! ==> access_token ::-----> {token.access_token},|||||||||   refresh_token ::----> {token.refresh_token}");
                    return Ok(token);
                }
                else
                {
                    Log.Error("Access token not recived!");
                    throw new Exception("Error occure when try to retrive access token! ");
                }
            }
            catch (Exception ex) 
            {
                Log.Error("User loging not succes!");
                throw new Exception("Error occure when try to loging ");
            }
            
        }


        //ViewUserInfo endpoint to view address of the user.
        /// <summary>
        /// Retrieves a user info.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the User info.</returns>
        [HttpGet("user-info"), Authorize]
        public async Task<IActionResult> ViewUserInfo()
        {
            try
            {
                var userInfo = await _addressService.ViewUserDetails();
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "UserInfo not found!", Error = ex.Message });
                throw new Exception("UserInfo not found!");
            }
        }



        // PostAddress endpoint to add a new address for the user.
        /// <summary>
        /// Adds a new address for the user.
        /// </summary>
        /// <param name="addressReq">The address data provided in the request body.</param>
        /// <returns>An IActionResult representing the result of the address addition process.</returns>
        [HttpPost("new-address"), Authorize]
        public async Task<IActionResult> PostAddress([FromBody] AddressDTO addressReq)
        {
            try
            {
                //call the AddAddress service with given address data
                var address = await _addressService.AddAddress(addressReq);

                Log.Information("New address added succesfully!");
                return Ok(address);

            }
            catch (Exception ex)
            {
                Log.Error("Error occured while add new address!");
                return BadRequest(ex.Message);
                throw new Exception("Error occured while add new address!");
            }
        }


        //ViewAddress endpoint to view address of the user.
        /// <summary>
        /// Retrieves a list of addresses.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the list of addresses.</returns>
        [HttpGet("address"), Authorize]
        public async Task<IActionResult> ViewAddresses()
        {
            try
            {
                var addresses = await _addressService.ViewAddress();
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Address not found!", Error = ex.Message });
                throw new Exception("Address not found!");
            }
        }



        /// <summary>
        /// Get new Access token from Refresh token
        /// </summary>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("/access-token-refresh-token")]
        public async Task<IActionResult> getrefreshToken(string refresh_token)
        {
            try
            {
                var result = await _authService.GetAccessTokenFromRefreshToken(refresh_token);
                return Ok(result);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
                
            }
        }

    }
}
