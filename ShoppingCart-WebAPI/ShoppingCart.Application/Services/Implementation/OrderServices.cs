using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Domain.Models;
using ShoppingCart.Application.Interfaces;
using Order = ShoppingCart.Domain.Models.Order;
using ShoppingCart.Application.Services.Interfaces;
using ShoppingCart.Application.Interfaces.IRepositories;
using System.Net;


namespace ShoppingCart.Application.Services.Implementation
{
    public class OrderServices : IOrderServices
    {
        // Dependencies
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenServices _tokenServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUpdateTimeStampService _updateTimeStampService;


        // Constructor
        public OrderServices(
            IHttpContextAccessor httpContextAccessor, 
            ITokenServices tokenServices, 
            IUnitOfWork unitOfWork,
            IUpdateTimeStampService updateTimeStampService) 
        {
            

            _unitOfWork = unitOfWork;
            _updateTimeStampService = updateTimeStampService;
            _httpContextAccessor = httpContextAccessor;
            _tokenServices = tokenServices;

        }




        // Get a new order
        public async Task<Order> GetOrder(double orderTotal, string orderStatus, string paymentID, string sessionId, string cutomerID)
        {
            // Retrieve user from database
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var user = await _unitOfWork.GetBaseRepository<User>().FirstOrDefaultAsyncByID(cutomerID, a => a.strip_CustomerID == cutomerID);

                // Throw exception if user not found
                if (user == null)
                {
                    throw new Exception("Customer not Found!");
                }

                var userID = user.userID;


                // Create new order
                var newOrder = new Order
                {
                    userID = userID,
                    orderDate = DateTime.Now,
                    orderTotal = orderTotal,
                    orderSatus = orderStatus,
                    paymentID = paymentID,
                    SessionID = sessionId
                };

                // Add new order to database
                var sample = await _unitOfWork.GetBaseRepository<Order>().AddAsync(newOrder);
                await _unitOfWork.SaveChangesAsync();


                // Log new order
                Log.Information("get new order => {@newOrder}", newOrder);



                // Get cart items from database
                var user_product1 =  _unitOfWork.GetBaseRepository<UserCart>().FindByID(userID, a => a.userID == userID).Include("Product");
                var user_product =  user_product1.ToList();



                // Process each cart item
                if (sample != null)
                {
                    //add cart item to ordereproduct table after payment success..
                    foreach (var Item in user_product)
                    {
                        // Create new order item
                        CurrentOrders orderItems = new CurrentOrders
                        {
                            orderID = newOrder.orderID,
                            productID = Item.productID,
                            unitPrice = Item.Product.price,
                            orderQuantity = Item.quantity

                        };

                        // Add order item to database
                        await _unitOfWork.GetBaseRepository<CurrentOrders>().AddAsync(orderItems);
                        await _unitOfWork.SaveChangesAsync();


                        // Update product stock

                        var orderStock = Item.quantity;
                        var newStock = Item.Product.stock - orderStock;
                        await _unitOfWork.GetBaseRepository<Product>().UpdateColumn(Item.productID, "stock", newStock);
                        await _unitOfWork.SaveChangesAsync();



                    }


                    // Clear user's cart
                     var clearCart =  _unitOfWork.GetBaseRepository<UserCart>().FindByID(userID, a => a.userID == userID).ToList();
                    await _unitOfWork.GetBaseRepository<UserCart>().RemoveRangeAsync(clearCart);
                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.CommitAsync();

                    Log.Information("cart clear, removed items => {@clearCart}", clearCart);
                    

                }
                return (newOrder);

            }
            catch
            {
                    await _unitOfWork.RollbackAsync();
                    throw new("Erro occued during try to get a order!");

            }


               

        }





        // View all orders of user
        public async Task<List<Order>> ViewAllOrders()
        {

            // Get user ID
            var user_ID = _tokenServices.GetUserIDClaim();



            if(user_ID == null )
            {
                throw new Exception("User Id not found!");
            }

            // Retrieve orders from database
            var result = await _unitOfWork.GetBaseRepository<Order>().GetByIDAsync(user_ID,a => a.userID == user_ID);


            // Throw exception if no orders found
            if (result == null)
            {
                Log.Error("order result null!");
                throw new("Don't have any order found!");

            }

            // Log and return orders
            Log.Information("List orders => {@result}", result);
            return result;
        }





        // Add product to cart (User Product)
        public async Task<string> AddToCart(UserProductDTO cartReq)
        {
            try
            { 
                await _unitOfWork.BeginTransactionAsync();
                
                // Get user ID from access token
                var user_ID = _tokenServices.GetUserIDClaim();

               
                // Create new cart item
                var newCart = new UserCart
                {
                    userID = user_ID,
                    productID = cartReq.productID,
                    quantity = cartReq.quantity,
                    createDate = DateTime.Now,
                    cartCreateDate = DateTime.Now,
                    cartUpdateDate = DateTime.Now
                };


                // Check if product exists
                var prodcutStock = await _unitOfWork.GetBaseRepository<Product>().FirstOrDefaultAsyncByID(cartReq.productID, a => a.productID == cartReq.productID);


                if (prodcutStock == null)
                {
                    Log.Error("product stock are null!");
                    throw new Exception("Not found any product of given product ID!");
                }

                // Check if quantity exceeds stock
                if (cartReq.quantity >= prodcutStock.stock && prodcutStock == null)
                {
                    Log.Error("given quantity exceeded current stock!");
                    throw new Exception("Cant add given number of items to cart!! given quantity out of stock! or Product not found!");
                }

                // Add new cart item to database
                await _unitOfWork.GetBaseRepository<UserCart>().AddAsync(newCart);
                await _unitOfWork.SaveChangesAsync();

                await _updateTimeStampService.updateDateTime(user_ID);
                await _unitOfWork.SaveChangesAsync();

                Log.Information("Add Item to cart => {@prodcutStock}", prodcutStock);
                await _unitOfWork.CommitAsync();
                return "Cart create succesfully!";

            }catch
            {
                await _unitOfWork.RollbackAsync();
                throw new("Error occured suring cart creating!");
            }

        }



        // View all cart items in cart
        public async Task<object> ViewCartItem()
        {

            // Get user ID
            var user_ID = _tokenServices.GetUserIDClaim();

            // Retrieve cart items from database
            var cartItemList = _unitOfWork.GetBaseRepository<UserCart>().FindByID(user_ID,a => a.userID == user_ID).Include("Product").ToList();


            // Throw exception if no cart items found
            if (cartItemList == null)
            {
                Log.Error("don't have any item in the cart!");
                throw new Exception("Don't have any cart item!");
            }


            // Map cart items to DTO
            var fullData = cartItemList.Select(a => new ProductCategoryDTO()
            {
                userID = user_ID,
                productID = a.productID,
                name = a.Product.name,
                price = a.Product.price,
                description = a.Product.description,
                imageURL = a.Product.imageURL,
                discount = a.Product.discount,
                stock = a.Product.stock,
                quantity = a.quantity,
                createDate = a.createDate,
                updateDate = a.updateDate,

            }).ToList();


            // Calculate total items and subtotal
            var totalItems = fullData.Count;


            DateTime? cartCreatedAt = null;
            DateTime? cartUpdatedAt = null;


            //create date and update date assign to cart
            if (cartItemList.Any())
            {
                var firstCartItem = _unitOfWork.GetBaseRepository<UserCart>()
                    .FindByID(user_ID,u => u.userID == user_ID)
                    .Select(up => new { up.cartCreateDate, up.cartUpdateDate })
                    .FirstOrDefault();

                if (firstCartItem != null)
                {
                    cartCreatedAt = firstCartItem.cartCreateDate;
                    cartUpdatedAt = firstCartItem.cartUpdateDate;
                }
            }



            var total = fullData.Sum(a => a.quantity * a.price);


            Log.Information("View cart items => {@fullData}", fullData);



            //I have changed the annonymus to cartinfoDTo type this return.
            return new CartInfoDTO { userID = user_ID, cartItems = fullData, cart_create_date = cartCreatedAt, cart_update_date = cartUpdatedAt, totalItems = totalItems, subTotal = total };
        }





        // Delete cart item by productID
        public async Task<UserCart> DeleteCartItem(int id)
        {
            try
            {

                await _unitOfWork.BeginTransactionAsync();
                // Get user ID
                string uID = _tokenServices.GetUserIDClaim();

                // Retrieve cart item from database
                 var item =  _unitOfWork.GetBaseRepository<UserCart>().FindByID(id, uID, a => a.productID == id && a.userID == uID).FirstOrDefault();
                var selectItem =  _unitOfWork.GetBaseRepository<UserCart>().FindByID(uID, a => a.userID == uID).FirstOrDefault();

                // Throw exception if cart item not found
                if (item != null)
                {
                    // Remove cart item from database
                    await _unitOfWork.GetBaseRepository<UserCart>().RemoveAsync(item);
                    await _updateTimeStampService.updateDateTime(uID);
                    //wait updateDateTime(uID);
                    await _unitOfWork.SaveChangesAsync();
                    Log.Information("View cart items => {@item}", item);
                    await _unitOfWork.CommitAsync();
                    return item;


                }
                else
                {
                    Log.Error("don't have item equal to given prodct id!");
                    throw new Exception("Item not Found!");
                }

                
            }catch
            {
                await _unitOfWork.RollbackAsync();
                throw new("Error occured during delete ");
            }


        }



        // Update cart item quantity
        public async Task<UserCart> UpdateCartItemQuantityAsync(int productId, int newQuantity)
        {
            
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get current logged-in user ID
                var curreLogedUser = _tokenServices.GetUserIDClaim();

                // Retrieve cart item from database
                var item = await _unitOfWork.GetBaseRepository<UserCart>()
                    .FindByID(productId, curreLogedUser, up => up.userID == curreLogedUser && up.productID == productId).FirstOrDefaultAsync();
            
                  
                // Throw exception if cart item not found
                if (item == null)
                {
                    Log.Error("cart item not found, productId : {@productID}", productId);

                    throw new Exception($"Cart item with product ID {productId} not found.");

                }

                // Retrieve product from database
                 var product = await _unitOfWork.GetBaseRepository<Product>().GetById(productId);

                // Throw exception if product not found
                if (product == null)
                {
                    Log.Error("Product not found!");
                    throw new Exception($"Product not found.");
                }

                // Check if quantity is valid
                if (newQuantity <= 0)
                {
                    Log.Error("Quantity not in correct format!");
                    throw new Exception("Invalid quantity.");
                }

                // Check if new quantity exceeds stock
                if (newQuantity > product.stock)
                {
                    Log.Error("Stock not enough!");
                    throw new Exception($"Out of stock. stock: {product.stock}");
                }

                // Update cart item quantity and update date
                item.quantity = newQuantity;


                item.updateDate = DateTime.Now;
                //await updateDateTime(curreLogedUser);
                await _updateTimeStampService.updateDateTime(curreLogedUser);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();


                Log.Information("Updated cart item quantity : {@productID}", productId);
                return item;
               
            }catch
            {
                await _unitOfWork.RollbackAsync();
                throw new("Error in updated cart item quantity");
            }

        }
    }
}
