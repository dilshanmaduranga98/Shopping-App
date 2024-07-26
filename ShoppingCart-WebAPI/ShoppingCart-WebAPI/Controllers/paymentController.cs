using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ShoppingCart.Domain.Helper;
using ShoppingCart.Infrastructure.Data;
using ShoppingCart.Application.Interfaces;
using Stripe;
using Stripe.Checkout;
using ShoppingCart.Application.Services.Interfaces;

namespace ShoppingCart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class paymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderServices _orderServices;
        public readonly IEmailInterface _emailInterface;
        public readonly ShoppingDbContext _shoppingDbContext;
        private readonly ILogger<paymentController> _logger;


        // Constructor injection for dependencies.
        public paymentController(IPaymentService paymentService, IOrderServices orderServices, IEmailInterface emailInterface, ShoppingDbContext shoppingDbContext, ILogger<paymentController> logger)
        {
            _paymentService = paymentService;
            _orderServices = orderServices;
            _emailInterface = emailInterface;
            _shoppingDbContext = shoppingDbContext;
            _logger = logger;
        }


        // Index endpoint to handle incoming webhooks.
        /// <summary>
        /// Handles incoming webhooks.
        /// </summary>
        /// <returns>An IActionResult representing the result of webhook processing.</returns>
        //webhook controller
        [HttpPost("webhook")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripEvent = EventUtility.ParseEvent(json);

                if (stripEvent.Type == Events.CheckoutSessionCompleted)
                {
                    // Handling checkout session completion event.
                    // Fetching session details and processing them.
                    var session = stripEvent.Data.Object as Stripe.Checkout.Session;
                    Console.WriteLine(session.Id);
                    var options = new SessionGetOptions();
                    options.AddExpand("line_items");
                    options.AddExpand("line_items.data.price.product");
                    var service = new SessionService();


                    // Retrieve the session. If you require line items in the response, you may include them by expanding line_items.
                    Session sessionWithLineItems = service.Get(session.Id, options);
                    StripeList<LineItem> lineItems = sessionWithLineItems.LineItems;


                    var orderTotal = session.AmountSubtotal;
                    var orderStatus = session.PaymentStatus;
                    var paymentId = session.PaymentIntentId;
                    var sessionId = session.Id;
                    var customerID = session.CustomerId;


                    //add order data to data base
                    var orderAdd = await _orderServices.GetOrder((double)orderTotal, orderStatus, paymentId, sessionId, customerID);
 

                    var userMail = await _emailInterface.getHtmlContent(orderAdd);

                    var toMail = userMail.User.email;
                    var toName = userMail.User.firstname;


                    var htmlContent = EmailContectHelper.CreateHtmlConent(userMail);

                    var subject = $"Payment Invoice of {toName}";
                    var palintText = "Thank you for shopping with us, here is your payment invoice.";

                     await _emailInterface.sendGridService(toMail, toName, htmlContent, subject, palintText);

                    Log.Information("email sending success!");



                }
                else if (stripEvent.Type == Events.PaymentMethodAttached)
                {
                    // Handling payment method attachment event.
                    var payamnetMethod = stripEvent.Data.Object as PaymentMethod;

                }
                Log.Information("Payment success!");
                return Ok();
                

            }
            catch (Exception ex)
            {
                Log.Error("Payment not success!");
                return BadRequest(ex.Message);
                throw new Exception("Payment not success!");
            }
        }



        // CheckoutSession endpoint to initiate the checkout session for processing payments.
        /// <summary>
        /// Initiates the checkout session for processing payments.
        /// </summary>
        /// <returns>An IActionResult representing the result of the checkout session initiation.</returns>
        //checkout session service
        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> CheckoutSession()
        {
            try
            {
                var checkout = await _paymentService.ItemCheckout();
                Log.Information("checkout start!");
                return Ok(new { Url = checkout });

            }
            catch (Exception ex)
            {
                Log.Error("Checkout not succeful!");
                return BadRequest();
                throw new Exception("Checkout not succeful!");
            }
        }



        // GetPaymentgHistory endpoint to retrieve the payment history for the authorized user.
        /// <summary>
        /// Retrieves the payment history for the authorized user.
        /// </summary>
        /// <returns>An IActionResult representing the payment history data.</returns>
        [HttpGet("payment-history")]
        [Authorize]
        public async Task<IActionResult> GetPaymentgHistory()
        {
            try
            {
                var result = await _paymentService.PaymentHistory();
                Log.Information("Payment history => {@result}", result);
                return Ok(result);


            }
            catch (Exception ex)
            {
                Log.Error("Error occured while try to get payment history!");
                return BadRequest(ex.Message);
                throw new Exception("Error occured while try to get payment history!");
            }
        }



        // ViewPaymentHistoryProducts endpoint to retrieve the list of paid items from the payment history based on the order ID.
        /// <summary>
        /// Retrieves the list of paid items from the payment history based on the order ID.
        /// </summary>
        /// <param name="orderId">The order ID for which to retrieve the paid items.</param>
        /// <returns>An IActionResult representing the list of paid items.</returns>
        //payment history item list by it's order id
        [HttpGet("paied-items/{orderId}")]
        [Authorize]
        public async Task<IActionResult> ViewPaymentHistoryProducts(int orderId)
        {
            try
            {
                var result = await _paymentService.PaymentHistoryProducts(orderId);
                Log.Information("payied product list =>{@result}", result);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "No orders found!", Error = ex.Message });
                throw new Exception("No orders found!");
            }

        }
    }
}
