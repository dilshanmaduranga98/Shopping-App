using ShoppingApp.FrontEnd.Models;

namespace ShoppingApp.FrontEnd.Interfaces
{
    public interface IProdcutServices
    {
        Task<List<ProductModel>> GetAllProducts();
        Task<List<ProductModel>> GetByCategory(int categoryID);
        Task<List<CategoryModel>> GetAllCategories();

        Task<ProductModel> GetProductDetails(int productID);
    }
}
