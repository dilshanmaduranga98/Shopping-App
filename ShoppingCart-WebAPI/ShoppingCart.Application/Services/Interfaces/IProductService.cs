using ShoppingCart.Application.DTOs;
using ShoppingCart.Domain.Models;



namespace ShoppingCart.Application.Services.Interfaces
{

    /// Interface for product-related services
    public interface IProductService
    {

        // Method to retrieve products by their category using category ID
        Task<List<Product>> GetProductByCategory(int ID);



        // Method to retrieve all products
        Task<List<Product>> ViewAllProducts();


        // Method to retrieve product by its product ID
        Task<ProductViewDTO> ViewProductByID(int ID);


        //Task<Product> AddProduct(ProductDTO productReq);

        //Task<ProductCategory> AddProductCategory(CategoryDTO categoryReq);


        //retrive all product category data from the database
        Task<List<ProductCategory>> ViewAllProductCategory();
    }
}
