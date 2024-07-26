
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Services.Interfaces;
using ShoppingCart.Domain.Models;
using ShoppingCart.Infrastructure.Data;


//all controllers regarding product management
namespace ShoppingCart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class productsController : ControllerBase
    {

        private readonly IProductService _productService;

        // Constructor injection for dependencies.
        public productsController(IProductService productService)
        {
            _productService = productService;
        }


       

        // GetAllProducts endpoint to retrieve all products from the database.
        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>An IActionResult representing the list of products.</returns>
        //view all products
        [HttpGet("managed-product/products")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var result = await _productService.ViewAllProducts();

                Log.Information("Product list displaied!");
                
                return Ok(result);
            }
            catch (Exception ex) 
            {
                Log.Error("Error occured while try to get product data!");

                return NotFound(new
                {
                    Error = "No Item founds!!"
                });
                throw new Exception("Error occured while try to get product data!");
            }
        }




        // GetByCategory endpoint to retrieve products from the database based on the specified category ID.
        /// <summary>
        ///  Retrieves products from the database based on the specified category ID.
        /// </summary>
        /// <param name="categoryID">Enter the category ID for filtering products.</param>
        /// <returns>An IActionResult representing the list of products.</returns>
        //view product by category
        [HttpGet("managed-category/{categoryID:int}")]
        public async Task<IActionResult> GetByCategory(int categoryID)
        {
            try
            {
                var result = await _productService.GetProductByCategory(categoryID);

                Log.Information("products found by category ID!");

                return Ok(result);

            }
            catch (Exception ex)
            {
                Log.Information("Error occured while try to get products data by category!");

                return BadRequest(ex.Message);
                throw new Exception("Error occured while try to get products data by category!");
            }
        }


        // GetProductByID endpoint to retrieve product information by its unique identifier.
        /// <summary>
        /// Retrieves product information by its unique identifier.
        /// </summary>
        /// <param name="productID">The ID of the product to retrieve.</param>
        /// <returns>An asynchronous task that returns an IActionResult.</returns>
        [HttpGet("managed-product/product/{productID:int}")]
        public async Task<IActionResult> GetProductByID(int productID)
        {
            try
            {
                var product = await _productService.ViewProductByID(productID);

                Log.Information("get product by it's ID!");

                return Ok(product);

            }
            catch (Exception ex)
            {
                Log.Error("Error occured while try to get product data by it's ID!");

                return BadRequest(ex.Message);
                throw new Exception("Error occured while try to get product data by it's ID!");

            }

        }





        //*********************************************************************************************************************
        //Remove Later
        //*********************************************************************************************************************


        ///// <summary>
        ///// Adds a new product to the database.
        ///// </summary>
        ///// <param name="productReq">The product data to be added.</param>
        ///// <returns>An IActionResult representing the result of the operation.</returns>
        ////add new product to dataBase
        //[HttpPost("managed-product")]
        //public async Task<IActionResult> Post([FromBody] ProductDTO productReq)
        //{
        //    try
        //    {
        //        var req = await _productService.AddProduct(productReq);
        //        return Ok(req);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}



        ///// <summary>
        ///// Adds a new category to the database.
        ///// </summary>
        ///// <param name="categoryReq">The category data to be added.</param>
        ///// <returns>An IActionResult representing the result of the operation.</returns>
        ////add new category to dataBase
        //[HttpPost("managed-category")]
        //public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryReq)
        //{
        //    if (categoryReq == null)
        //    {
        //        return BadRequest(new
        //        {
        //            error = "Request null or empty!"
        //        });
        //    }

        //    try
        //    {
        //        var postCategory = await _productService.AddProductCategory(categoryReq);
        //        return Ok(new
        //        {
        //            message = "category create succefully!",
        //            Category_Data = postCategory
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        //*********************************************************************************************************************
        //*********************************************************************************************************************


        /// <summary>
        /// View all category data from database.
        /// </summary>
        /// <param></param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        //View all category data from dataBase
        [HttpGet("managed-category/catgories")]
        public async Task<IActionResult> ViewCategory()
        {

            try
            {
                var ViewAllCategory = await _productService.ViewAllProductCategory();
                return Ok(ViewAllCategory);
            }
            catch (Exception ex)
            {
                Log.Error("Error occured while try to get category data!");
                return NotFound("No category founds!!");
                throw new Exception("Error occured while try to get category data!");

                
            }
        }

    }
}
