using ShoppingApp.FrontEnd.Interfaces;
using ShoppingApp.FrontEnd.Models;
using System.Net.Http.Json;


namespace ShoppingApp.FrontEnd.Services
{
    public class ProductServices : IProdcutServices
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductServices"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to make API requests.</param>
        public ProductServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        //get all products as list
        /// <summary>
        /// Retrieves all products from the API.
        /// </summary>
        /// <returns>List of ProductModel if successful; otherwise, null.</returns>
        public async Task<List<ProductModel>> GetAllProducts()
        {
            var result = await _httpClient.GetFromJsonAsync<List<ProductModel>>("https://localhost:7036/api/products/managed-product/products");
            
            if(result != null) 
            {
                return result;
            }else
            {
                return null;
            }
            
        }


        //get products data by it's categoryID
        /// <summary>
        /// Retrieves products filtered by category ID from the API.
        /// </summary>
        /// <param name="categoryID">The ID of the category to filter products.</param>
        /// <returns>List of ProductModel if successful; otherwise, null.</returns>
        public async Task<List<ProductModel>> GetByCategory(int categoryID)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ProductModel>>($"https://localhost:7036/api/products/managed-category/{categoryID}");
            if(result != null) 
            {
                return result;
            }else
            {
                return null;
            }
        }


        //get all Catgories as list
        /// <summary>
        /// Retrieves all categories from the API.
        /// </summary>
        /// <returns>List of CategoryModel if successful; otherwise, null.</returns>\
        public async Task<List<CategoryModel>> GetAllCategories()
        {
            var result = await _httpClient.GetFromJsonAsync<List<CategoryModel>>("https://localhost:7036/api/products/managed-category/catgories");
            

            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }

        }



        //get product details by it's ID
        /// <summary>
        /// Retrieves product details by its ID from the API.
        /// </summary>
        /// <param name="productID">The ID of the product to retrieve details for.</param>
        /// <returns>ProductModel if successful; otherwise, null.</returns>
        public async Task<ProductModel> GetProductDetails(int productID)
        {
            var result = await _httpClient.GetFromJsonAsync<ProductModel>($"https://localhost:7036/api/products/managed-product/product/{productID}");

            if (result != null)
            {
                
                return result;
                
            }
            else
            {
                return null;
            }
        }
    }
}
