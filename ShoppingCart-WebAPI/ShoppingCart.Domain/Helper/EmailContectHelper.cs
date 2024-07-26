using ShoppingCart.Domain.Models;
using System.Text;

namespace ShoppingCart.Domain.Helper
{

    // Helper class for email content generation.
    public class EmailContectHelper
    {


        // Method to create a welcome email template.
        public static string CreateWelcomeEmail(User user)
        {

            // Creating a StringBuilder to construct the email content.
            StringBuilder emailContent = new StringBuilder();


            // Appending HTML content for the email.
            emailContent.AppendLine("<html>");
            emailContent.AppendLine("<head>");
            emailContent.AppendLine("<meta charset=\"UTF-8\">");
            emailContent.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            emailContent.AppendLine("<title>Welcome to Our Shopping App!</title>");

            emailContent.AppendLine("<style>");
            emailContent.AppendLine("body { font-family: Arial, sans-serif;  background-color: #f5f5f5; width: 100%; }");
            emailContent.AppendLine(".container {  max-width: 600px;  margin: 0 auto;  padding: 20px;  background-color: #fff;  border-radius: 8px;  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);  object-fit: con; text-align: center; }");
            emailContent.AppendLine("h1, h2 { text-align: center;  color: #333; }");
            emailContent.AppendLine("p {  color: #9e9e9e;  z-index: 100; text-align: center; font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }");
            emailContent.AppendLine(".messg-topic { color: #f3f3f3;  z-index: 100; text-align: center; font-size: 35px; font-weight: 600; position: absolute; bottom: 50%; left :0; padding: 0 40px; line-height: 18px; font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }");
            emailContent.AppendLine(".messg { color: #4d4d4d;  z-index: 100; text-align: center; padding: 0 40px; line-height: 22px; font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }");
            emailContent.AppendLine(".x { line-height: 25px; }");
            emailContent.AppendLine(".highlight {  color: #0092ac;  font-weight: bold; }");
            emailContent.AppendLine(".headding-sec { background-color:transparent;    color: #ffffff;  font-weight: bold;  padding: 25px 10px;  font-size: 40px;  z-index: 100; }");
            emailContent.AppendLine(".welcome { text-align: center; padding: 5px; background-color: #000000c9; width: auto; height: 100%; z-index: 100; }");
            emailContent.AppendLine(".welcome p { color: #fff; }");
            emailContent.AppendLine(".welcome-image { width: auto; background-image: url('https://img.freepik.com/free-photo/shopping-bags-various-colors-with-shopping-cart_23-2148288211.jpg?t=st=1716568729~exp=1716572329~hmac=e36ba790bc99727d0e68d7ddb66f8858d109a6b6c307c0e66890e4cf60ab6411&w=740'); background-size: cover; background-position: bottom; min-height: 600px;  position: relative; text-align: center; padding: 20px 20px; }");
            emailContent.AppendLine(".hr { width: 100%; height: 1px; background-color: #e7e7e7 !important; }");
            emailContent.AppendLine(".copyright { text-align: center; font-size: 10px; }");
            emailContent.AppendLine(".footer { color: #818181; }");
            emailContent.AppendLine(".name { color: #0092ac; }");
            emailContent.AppendLine("</style>");

            emailContent.AppendLine("</head>");
            emailContent.AppendLine("<body>");

            emailContent.AppendLine("<div class=\"container\">");

            emailContent.AppendLine("<div class=\"welcome-image\">");
            emailContent.AppendLine("<h1 class=\"headding-sec\">Welcome!</h1>");
            emailContent.AppendLine("<div class=\"welcome\">");
            emailContent.AppendLine($"<p class=\"messg\">Dear <span class=\"highlight\">{user.firstname}</span>,</p>");
            emailContent.AppendLine("<p class=\"messg\">Welcome to our platform! We are thrilled to have you join us.</p>");
            emailContent.AppendLine("<p class=\"messg x\">With our platform, you can enjoy a seamless shopping experience with a wide variety of products, competitive prices, fast checkout, and personalized recommendations. Benefit from exclusive deals, reliable customer support, and a hassle-free return policy for an enjoyable online shopping experience..</p>");
            emailContent.AppendLine("<p class=\"messg\"></p>");
            emailContent.AppendLine("<p class=\"messg\">Until then, enjoy your shopping!</p>");
            emailContent.AppendLine("</div>");
            emailContent.AppendLine("</div>");

            emailContent.AppendLine("<div class=\"hr\"></div>");
            emailContent.AppendLine("<p class=\"footer\">If you need any assistance, feel free to contact our support team.</p>");
            emailContent.AppendLine("<p class=\"footer\">Thank you !</p>");
            emailContent.AppendLine("<p class=\"copyright\">The Shopping Bay Team @2024 All Right Reserved.</p>");

            emailContent.AppendLine("</div>");

            emailContent.AppendLine("</body>");
            emailContent.AppendLine("</html>");


            // Returning the constructed email content as a string.
            return emailContent.ToString();
        }





        // Method to create an invoice email template.
        public static string CreateHtmlConent(Order order)
        {

            // Creating a StringBuilder to construct the invoice content.
            StringBuilder invoiceContent = new StringBuilder();

            // Appending HTML content for the invoice.
            invoiceContent.AppendLine("<html>");
            invoiceContent.AppendLine("<head>");
            invoiceContent.AppendLine("<meta charset=\"UTF-8\">");
            invoiceContent.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            invoiceContent.AppendLine("<title>Invoice from [Your Shopping Application Name]</title>");

            invoiceContent.AppendLine("<style>");
            invoiceContent.AppendLine("body { font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #ccc; }");
            invoiceContent.AppendLine(".container { max-width: 600px; margin: auto; padding: 20px; background-color: #fff; }");
            invoiceContent.AppendLine("h1, h2, h3, h4, p { margin: 0;  padding: 0; }");
            invoiceContent.AppendLine("h1 { text-align: center; margin-bottom: 20px; }");
            invoiceContent.AppendLine("table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            invoiceContent.AppendLine("th, td { padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }");
            invoiceContent.AppendLine("th { background-color: #f2f2f2; }");
            invoiceContent.AppendLine(".headding { background-color: #ffa631; padding: 20px; color: #fff; }");
            invoiceContent.AppendLine(".orderinfo { padding: 20px 0; margin-top: 0px; }");
            invoiceContent.AppendLine(".ordersummery{ padding: 20px 0; margin-top: 50px; }");
            invoiceContent.AppendLine(".footer-one { margin-top: 5px; margin-bottom: 5px; line-height: 18px; font-size: 0.8em; }");
            invoiceContent.AppendLine(".all-right {text-align: center; font-size: 10px; width: 100%; margin-top: 30px; color: #ffffff; padding: 5px 0; background-color: #ffa631; }");
            invoiceContent.AppendLine(".address-img { margin-top: 40px; }");
            invoiceContent.AppendLine(".address { font-size: 12px; }");
            invoiceContent.AppendLine(".top-img { width: 100%; height: 240px; }");
            invoiceContent.AppendLine(".top-img img {  width: 100%; height: 200px; object-fit: contain; }");
            invoiceContent.AppendLine(".header-line{ text-align: center; font-size: 1.2em; font-weight: 100;  padding: 2px; }");
            invoiceContent.AppendLine(".lone { margin-top: 40px; }");
            invoiceContent.AppendLine(".line{ width: 100%; height: 1px; background-color: #e7e7e7; margin-top: 40px; }");
            invoiceContent.AppendLine(".head-img { text-align: center; width: 100%; height: 100px; padding: 10px 0; }");
            invoiceContent.AppendLine(".head-img img { text-align: center; height: 100px; }");
            invoiceContent.AppendLine("</style>");

            invoiceContent.AppendLine("</head>");
            invoiceContent.AppendLine("<body>");
            invoiceContent.AppendLine("<div class=\"container\">");
            invoiceContent.AppendLine("<h1 class=\"headding\">Invoice from Shopping App</h1>");
            invoiceContent.AppendLine("<p class=\"header-line lone\">Your order is on the way! </p>");
            invoiceContent.AppendLine("<div class=\"head-img\">");
            invoiceContent.AppendLine("<img src=\"https://www.shutterstock.com/image-vector/fast-moving-shipping-delivery-truck-600nw-1202562907.jpg\"/>");
            invoiceContent.AppendLine("</div>");
            invoiceContent.AppendLine("<p class=\"header-line\"> We're preparing it for collection and can't wait for you to enjoy it. </p>");
            invoiceContent.AppendLine("<div class=\"line\"></div>");
            invoiceContent.AppendLine("<h2 class=\"orderinfo\">Order Information</h2>");
            invoiceContent.AppendLine("<table>");
            invoiceContent.AppendLine("<tr>");
            invoiceContent.AppendLine("<th>Order ID</th>");
            invoiceContent.AppendLine($"<td>{order.orderID}</td>");
            invoiceContent.AppendLine("</tr>");
            invoiceContent.AppendLine("<tr>");
            invoiceContent.AppendLine("<th>Order Date</th>");
            invoiceContent.AppendLine($"<td>{order.orderDate}</td>");
            invoiceContent.AppendLine("</tr>");
            invoiceContent.AppendLine("<tr>");
            invoiceContent.AppendLine("<th>Order status</th>");
            invoiceContent.AppendLine($"<td>{order.orderSatus}</td>");
            invoiceContent.AppendLine("</tr>");
            invoiceContent.AppendLine("<tr>");
            invoiceContent.AppendLine("<th>Customer Name</th>");
            invoiceContent.AppendLine($"<td>{order.User.firstname}</td>");
            invoiceContent.AppendLine("</tr>");
            invoiceContent.AppendLine("<tr>");
            invoiceContent.AppendLine("<th>Email</th>");
            invoiceContent.AppendLine($"<td>{order.User.email}</td>");
            invoiceContent.AppendLine("</tr>");
            invoiceContent.AppendLine("</table>");
            invoiceContent.AppendLine("<table>");
            invoiceContent.AppendLine("<tr>");
            invoiceContent.AppendLine("<th>Product</th>");
            invoiceContent.AppendLine("<th>Price</th>");
            invoiceContent.AppendLine("<th>Quantity</th>");
            invoiceContent.AppendLine("<th>Total</th>");
            invoiceContent.AppendLine("</tr>");


            if (order.CurrentOrders is not null)
            {
                // Iterating through each item in the order's current orders collection.
                foreach (var item in order.CurrentOrders)
                {
                    // Appending HTML table row for each item to the invoice content.
                    invoiceContent.AppendLine($"<tr><td>{item.Product?.name}</td><td>${item.unitPrice}</td><td>{item.orderQuantity}</td><td>${item.unitPrice * item.orderQuantity}</td></tr>");
                }
            }

            invoiceContent.AppendLine("<tr>");
            invoiceContent.AppendLine("<th colspan=\"3\">Total Amount</th>");

            // Total amount
            var total = order.orderTotal/100;
            invoiceContent.AppendLine($"<td>${total}</td>");
            invoiceContent.AppendLine("</tr>");
            invoiceContent.AppendLine("</table>");
            invoiceContent.AppendLine("<p class=\"footer-one\">Thank you for choosing Shopping App!</p>");
            invoiceContent.AppendLine("<p class=\"footer-one\">If you have any questions or concerns regarding your order or this invoice, please feel free to contact our customer support team.</p>");
            invoiceContent.AppendLine("<div class=\"line\"></div>");
            invoiceContent.AppendLine("<img class=\"address-img\" src=\"https://img.freepik.com/free-vector/hand-drawn-shop-local-logo-design_23-2149575772.jpg\" width=\"120px\"/>");
            invoiceContent.AppendLine("<p class=\"address\">Shopping App pvt(Ltd).</p>");
            invoiceContent.AppendLine("<p class=\"address\">1st lane,</p>");
            invoiceContent.AppendLine("<p class=\"address\">Kiribathgoda,</p>");
            invoiceContent.AppendLine("<p class=\"address\">Colombo,</p>");
            invoiceContent.AppendLine("<p class=\"address\">Sri Lanka.</p>");
            invoiceContent.AppendLine("<p class=\"address\">test@shoppingapp.com</p>");
            invoiceContent.AppendLine("<p class=\"address\">+94-77-154-2853</p>");
            invoiceContent.AppendLine("<p class=\"all-right\">Shoppping app @2024. All Right Reserved</p>");
            invoiceContent.AppendLine("</div>");
            invoiceContent.AppendLine("</body>");
            invoiceContent.AppendLine("</html>");


            // Returning the constructed invoice content as a string.
            return invoiceContent.ToString(); 

        }

    }
}
