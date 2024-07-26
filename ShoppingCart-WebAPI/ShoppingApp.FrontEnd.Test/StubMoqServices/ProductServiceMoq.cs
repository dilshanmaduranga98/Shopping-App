using ShoppingApp.FrontEnd.Interfaces;
using ShoppingApp.FrontEnd.Models;


namespace ShoppingApp.FrontEnd.Test.StubMoqServices
{
    
    public class ProductServiceMoq : IProdcutServices
    {
        public Task<List<CategoryModel>> GetAllCategories()
        {
            // Return hardcoded categories for testing
            return Task.FromResult(new List<CategoryModel>
            {
                new CategoryModel { 
                    categoryID = 1, 
                    name = "Category 1",
                    imageURL = "image"
                },
                new CategoryModel { 
                    categoryID = 2, 
                    name = "Category 2",
                    imageURL = "image"
                },
                // Add more as needed
            });
        }

        public Task<List<ProductModel>> GetAllProducts()
        {
            // Return hardcoded products for testing
            return Task.FromResult(new List<ProductModel>
            {
                new ProductModel {
                    productID = 1,
                    name = "Product 1",
                    description="test",
                    imageURL="image",
                    discount = 10,
                    price = 10,
                    categoryID = 1,
                },
                new ProductModel {
                    productID = 2,
                    name = "Product 2",
                     description="test",
                    imageURL="image",
                    discount = 10,
                    price = 10,
                    categoryID = 1,
                },
                 
            }
            );
        }

        public Task<List<ProductModel>> GetByCategory(int categoryID)
        {
            // Return hardcoded products by category for testing
            return Task.FromResult(new List<ProductModel>
            {
                 new ProductModel {
                    productID = 1,
                    name = "Product 1",
                    description="test",
                    imageURL="image",
                    discount = 10,
                    price = 10,
                    categoryID = 1,
                },
                new ProductModel {
                    productID = 2,
                    name = "Product 2",
                     description="test",
                    imageURL="image",
                    discount = 10,
                    price = 10,
                    categoryID = 1,
                },
                // Add more as needed
            });
        }

        public Task<ProductModel> GetProductDetails(int productID)
        {
            // Return hardcoded product details for testing
            return Task.FromResult(new ProductModel
            {
                productID = productID,
                name = $"Product {productID}",
                description = "test",
                imageURL = "image",
                discount = 10,
                price = 10,
                categoryID = 1,
               
                // Add more properties as needed
            });
        }
    }
}
