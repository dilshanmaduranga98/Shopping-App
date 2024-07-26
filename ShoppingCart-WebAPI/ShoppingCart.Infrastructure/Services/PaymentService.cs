using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Infrastructure.Data;
using ShoppingCart.Application.Interfaces;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using ShoppingCart.Infrastructure.AuthServicec;
using Microsoft.Extensions.Options;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Domain.Models;



namespace ShoppingCart.Infrastructure.Services
{



    public class PaymentService : IPaymentService
    {
       public readonly ShoppingDbContext _shoppingDbContext;        


        public readonly ITokenServices _tokenServices;
        public readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PaymentService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly KeyConfigurations _keyConfigurations;


        public PaymentService(
            ITokenServices tokenServices,
            IHttpContextAccessor contextAccessor,
            ILogger<PaymentService> logger,
            IOptions<KeyConfigurations> options, 
            IUnitOfWork unitOfWork)
        {
            
            _tokenServices = tokenServices;
            _httpContextAccessor = contextAccessor;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _keyConfigurations = options.Value;
        }

        //create stripe customer service
        public async Task<Customer> CreateCustomer(string name, string email)
        {
            // Setting Stripe API key
            StripeConfiguration.ApiKey = _keyConfigurations.StripeKey;

            // Creating options for customer creation
            var customer = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
            };

            var service = new CustomerService();
            var result = await service.CreateAsync(customer);

            Log.Information("Customer created => {@result}", result);
            return result;
        }


        //Item checkout service
        public async Task<string> ItemCheckout()
        {
            StripeConfiguration.ApiKey = _keyConfigurations.StripeKey;
            var userid = _tokenServices.GetUserIDClaim();

            // Retrieving user from database
            
            var user = await _unitOfWork.GetBaseRepository<User>().FirstOrDefaultAsyncByID(userid,a => a.userID == userid);

            // Retrieving user's carts with product details
           
            var userCarts = _unitOfWork.GetBaseRepository<UserCart>().FindByID(userid, a => a.userID == userid).Include("Product").ToList();
            
      

                //find related user is null or not
                if (user != null)
                {
                    var customerID = user.strip_CustomerID;

                    // Initializing list of line items for checkout
                    var lineItems = new List<SessionLineItemOptions>();


                    //UserProduct userProduct = new UserProduct();
                    foreach (var productItem in userCarts)
                    {
                        if (productItem.quantity > productItem.Product.stock) 
                        {
                            return "Item stock are not equal to requested quantity!";
                        }

                        // Creating line item for checkout
                        var lineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long?)(productItem.Product.price*100), // Use the unit price from the product table
                                Currency = "lkr",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = productItem.Product.name, // Use the product name from the product table
                                },
                            },
                            Quantity = productItem.quantity,
                        };
                        // Adding line item to list
                        lineItems.Add(lineItem);
                        
                    }


                    // Create the session options with the dynamic line items
                    var options = new SessionCreateOptions
                    {
                        LineItems = lineItems,
                        Mode = "payment",
                        SuccessUrl = "https://localhost:7252/thank-you",
                        CancelUrl = "https://www.marvel.com/404l",
                        Customer = customerID
                    };

                    var service = new SessionService();

                    // Creating session
                    Session session = service.Create(options);

                // Starting new process to open session URL

                //importent part ***********************************************

                        //Process.Start(new ProcessStartInfo("cmd", $"/c start {session.Url}")
                        //{
                        //    CreateNoWindow = true
                        //});

                //importent part ***********************************************



                Log.Information("Order checkout succesfully!");
                    return session.Url;
                }

                if (user == null)
                {
                    Log.Error("User not found!");
                    throw new Exception
                    (
                        message: "User not found!"
                    );
                }

                return " ";


        }


        


        //payment history retrive service as order list
        public async Task<List<PaymentHistoryDTO>> PaymentHistory()
        {
            var user = _tokenServices.GetUserIDClaim();

            var payments =  _unitOfWork.GetBaseRepository<Order>().FindByID(user,a => a.userID == user).Select(a => new PaymentHistoryDTO 
            { 
                order_id = a.orderID, 
                order_date = a.orderDate, 
                order_status = a.orderSatus, 
                total = a.orderTotal
            }).ToList();

            if(payments != null)
            {
                Log.Information("payments => {@payments}", payments);
                return payments;

            }else
            {
                Log.Error("don't have any payment data!");
                throw new Exception("No payments found!!");
            }

        }


        //payment history retrieve as product detail of each order by order id
        public async Task<List<PayiedProductDTO>> PaymentHistoryProducts(int orderId)
        {
            var userId = _tokenServices.GetUserIDClaim();
            
            var orderData =  _unitOfWork.GetBaseRepository<Order>().FindByID(orderId,a => a.orderID == orderId).ToList();


            if(orderData == null)
            {
                throw new Exception("No ordeer found!");
            }
            // Retrieving product list from database based on order ID
            var productList =  _unitOfWork.GetBaseRepository<Order>().FindByID(orderId,o => o.orderID == orderId)
                .Include(a => a.CurrentOrders)
                .ThenInclude(op => op.Product)
                .FirstOrDefault(o => o.orderID == orderId);


            // Checking if product list is null
            if (productList == null)
            {
                Log.Information("Don't have an any order!");
                throw new Exception("Don't have any product details!");
            }


            // Mapping product details to DTOs
            List<PayiedProductDTO> products = productList.CurrentOrders.Select(a => new PayiedProductDTO { name = a.Product.name, imageURL = a.Product.imageURL, price = a.Product.price, quantity = a.orderQuantity, totalPrice = (a.Product.price * a.orderQuantity) }).ToList();

            Log.Information("Payment history => {@products}", products);
            return products;


        }

    }

    
}




