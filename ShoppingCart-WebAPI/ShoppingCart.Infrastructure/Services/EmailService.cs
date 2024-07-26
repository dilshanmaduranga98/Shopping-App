using SendGrid.Helpers.Mail;
using SendGrid;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Infrastructure.AuthServicec;
using Microsoft.Extensions.Options;
using ShoppingCart.Domain.Models;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using Serilog;




namespace ShoppingCart.Infrastructure.Services
{
    public class EmailService : IEmailInterface
    {
       private readonly KeyConfigurations _keyConfigurations;
        //private readonly IBaseRepository<Order> _orderBaseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IOptions<KeyConfigurations> options, 
            IUnitOfWork unitOfWork)
        {
            _keyConfigurations = options.Value;
           // _orderBaseRepository = orderBaseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task sendGridService(string toMail, string toName, string emailHtmlContent, string emailSubject, string plainText)
        {
            // Initialize SendGrid client with API key
            var client = new SendGridClient(_keyConfigurations.SendGridKey);

            // Set email details
            var from = new EmailAddress("tharushathejanofficial@gmail.com", "Shopping App");
            var subject = emailSubject;
            var to = new EmailAddress(toMail, toName);
            var plainTextContent = plainText;
            var htmlContent = emailHtmlContent;

             
            // Create email message
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            // Send email
            var response = await client.SendEmailAsync(msg);
            Log.Information($"email send! {response}");

        }


        public async Task<Order> getHtmlContent(Order orderData)
        {
            var orderID = orderData.orderID;
      
            
            var htmlContent = await _unitOfWork.GetBaseRepository<Order>().FindByID(orderID,p => p.orderID == orderID).Include("User")
                .Include(a => a.CurrentOrders)
                .ThenInclude(ap => ap.Product)
                .FirstOrDefaultAsync();

            return htmlContent;
        }
    }
}
