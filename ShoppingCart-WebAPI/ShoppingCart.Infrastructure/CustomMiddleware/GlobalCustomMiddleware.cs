using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Web.Http.ExceptionHandling;
using Serilog;
using System.Net;
using ShoppingCart.Application.DTOs;

namespace ShoppingCart.Infrastructure.CustomMiddleware
{
    public class GlobalCustomMiddleware 
    {

        private readonly RequestDelegate _next;

        public GlobalCustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        // This method is called by the runtime to handle each HTTP request.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {

                // Call the next middleware in the pipeline
                await _next(context);

            }catch (Exception ex) 
            {
                // Set HTTP status code to 500 (Internal Server Error)
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Handle exceptions based on the request path
                if (context.Request.Path.StartsWithSegments("/signin"))
                {
                    // If the exception occurred in the "/signin" endpoint, rethrow it to propagate the original behavior
                    throw;
                }
                else
                {
                    // Create an error message object to send as JSON response
                    var errorMessage = new ErrorMessageDTO
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = ex.Message
                    };

                    // Serialize the error message object to JSON format
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(errorMessage);

                    // Set the response content type to JSON
                    context.Response.ContentType = "application/json";


                    // Write the JSON response to the HTTP response body
                    await context.Response.WriteAsync(json);

                    // Log the exception using Serilog
                    Log.Error(ex, "An error occured: {message}", json);
                }
            }
            
        }
    }
}
