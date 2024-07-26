

using ShoppingCart.Domain.Models;

namespace ShoppingCart.Application.Interfaces
{

    // Interface for email services
    public interface IEmailInterface
    {

        
        //get html content of the email body
        Task<Order> getHtmlContent(Order orderData);

        //Method for sending emails
        Task sendGridService(string toMail, string toName, string emailHtmlContent, string emailSubject, string plainText);

    }
}
