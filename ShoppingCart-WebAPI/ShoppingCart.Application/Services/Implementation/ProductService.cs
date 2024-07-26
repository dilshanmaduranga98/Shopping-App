using Microsoft.Extensions.Logging;
using Serilog;
using ShoppingCart.Application.Services.Interfaces;
using ShoppingCart.Application.Interfaces.IRepositories;
using ShoppingCart.Domain.Models;
using ShoppingCart.Application.DTOs;


namespace ShoppingCart.Application.Services.Implementation
{
    public class ProductService:IProductService
    {


        //private readonly IBaseRepository<Product> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;



        // Constructor to initialize ProductService with required dependencies.
        public ProductService( 
            IUnitOfWork unitOfWork)
        {
          
           // _baseRepository = baseRepository;
            _unitOfWork = unitOfWork;
        }



        // Service method to retrieve products by a specific category.
        public async Task<List<Product>> GetProductByCategory(int categoryID)
        {
            // Retrieving products by category ID.
           
            var result = await _unitOfWork.GetBaseRepository<Product>().GetByIDAsync(categoryID, a => a.categoryID == categoryID);


            if (result == null)
            {
                Log.Error("Don't have any category match given category ID!");
                throw new Exception("No any category found!");
            }

            Log.Information("Category data by ID => {@result}", result);

            // Returning the list of products.
            return result;

        }


        // Service method to view all products.
        public async Task<List<Product>> ViewAllProducts()
        {
            // Retrieving all products from the database.
            var allProducts = await _unitOfWork.GetBaseRepository<Product>().GetAll();

            if (allProducts == null)
            {

                Log.Error("Don't have any product in dataBase!");
                throw new Exception("Don't have any product details!!");
            }
            else
            {
                Log.Information("Products List => {@allProducts}", allProducts);

                // Returning the list of all products.
                return allProducts;
            }

        }

        // Service method to view product by its ID.
        public async Task<ProductViewDTO> ViewProductByID(int ID)
        {
            // Retrieving product by its ID.
             var product = await _unitOfWork.GetBaseRepository<Product>().GetById(ID);

            if (product == null)
            {
                Log.Error("Don't have any product match to given ID!");
                throw new Exception("Item Not found!!");
            }

            var productItem = new ProductViewDTO
            {
                // Mapping product properties to ProductViewDTO.
                Id = product.productID,
                name = product.name,
                description = product.description,
                imageURL = product.imageURL,
                price = product.price,
                discount = product.discount,
                stock = product.stock,
                categoryID = product.categoryID
            };

            Log.Information("Product data => {@productItem}", productItem);

            // Returning the product data.
            return productItem;
        }



        //*********************************************************************************************************************
        //Remove Later
        //*********************************************************************************************************************


        // Service method to add a new product to the database.
        //public async Task<Product> AddProduct(ProductDTO productReq)
        //{
        //    var msg = new Product
        //    {
        //        // Mapping productDTO properties to Product model.
        //        name = productReq.name,
        //        description = productReq.description,
        //        imageURL = productReq.imageURL,
        //        price = productReq.price,
        //        discount = productReq.discount,
        //        stock = productReq.stock,
        //        categoryID = productReq.categoryID,
        //    };

        //    // Adding the new product to the database.
        //    await _unitOfWork.GetBaseRepository<Product>().AddAsync(msg);
        //    await _unitOfWork.SaveChangesAsync();

        //    Log.Information("Add new Product, product data => {@msg}", msg);

        //    // Returning the added product.
        //    return msg;

        //}



        //// Service method to add a new product category to the database.
        //public async Task<ProductCategory> AddProductCategory(CategoryDTO categoryReq)
        //{
        //    var newCategory = new ProductCategory
        //    {
        //        // Mapping categoryDTO properties to ProductCategory model.
        //        name = categoryReq.name,
        //        imageURL = categoryReq.imageURL,
        //    };

        //    await _unitOfWork.GetBaseRepository<ProductCategory>().AddAsync(newCategory);
        //    await _unitOfWork.SaveChangesAsync();

        //    Log.Information("Add product category,  category data => {@newCategory}", newCategory);

        //    // Returning the added category.
        //    return newCategory;
        //}



        //*********************************************************************************************************************
        //*********************************************************************************************************************


        // Service method to add a new product category to the database.
        public async Task<List<ProductCategory>> ViewAllProductCategory()
        {

            List<ProductCategory> categoryList = await _unitOfWork.GetBaseRepository<ProductCategory>().GetAll();
            await _unitOfWork.SaveChangesAsync();

            Log.Information("View all product category,  category data => {@categoryList}", categoryList);

            // Returning the added category.
            return categoryList;
        }

    }
}
