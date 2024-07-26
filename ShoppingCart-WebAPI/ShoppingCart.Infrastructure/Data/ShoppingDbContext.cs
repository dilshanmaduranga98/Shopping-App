using Microsoft.EntityFrameworkCore;
using ShoppingCart.Domain.Models;

namespace ShoppingCart.Infrastructure.Data
{


    // DbContext class for interacting with the database
    public class ShoppingDbContext :DbContext
    {

        // Constructor to initialize DbContext with provided options
        public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options):base(options) 
        {
        }



        // Define DbSets for each entity representing database tables
        public DbSet<User> User { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<UserCart> UserCart { get; set; }
        public DbSet<CurrentOrders> CurrentOrders { get; set; }


        // Configure the database schema and relationships between entities
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Define configuration for the User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(a => a.userID);
                entity.Property(a => a.firstname).IsRequired();
                entity.Property(a => a.email).IsRequired();
                entity.Property(a => a.lastname).IsRequired();


            });

            // Define configuration for the Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(a => a.orderID);

                // Define relationship between Order and User entities
                entity.HasOne(a => a.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(a => a.userID)
                .OnDelete(DeleteBehavior.NoAction);

            });


            // Define configuration for the Address entity
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.addressID);

                // Define relationship between Address and User entities
                entity.HasOne(a => a.User)
                .WithMany(p => p.Addresss)
                .HasForeignKey(a => a.userID)
                .OnDelete(DeleteBehavior.NoAction);
            });

            // Define configuration for the Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(a => a.productID);

                // Define relationship between Product and ProductCategory entities
                entity.HasOne(a => a.ProductCategory)
                .WithMany(p => p.Products)
                .HasForeignKey(a => a.categoryID)
                .OnDelete(DeleteBehavior.NoAction);
            });


            // Define configuration for the ProductCategory entity
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(a => a.categoryID);

            });


            // Define configuration for the UserCart entity
            modelBuilder.Entity<UserCart>(entity =>
            {
                entity.HasKey(up => new
                {
                    up.userID,
                    up.productID
                });

                // Define relationship between UserCart, User, and Product entities
                entity.HasOne(up => up.User)
                .WithMany(a => a.UserCart)
                .HasForeignKey(up => up.userID)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(up => up.Product)
                .WithMany(a => a.UserCart)
                .HasForeignKey(up => up.productID)
                .OnDelete(DeleteBehavior.NoAction);
            });


            // Define configuration for the CurrentOrders entity
            modelBuilder.Entity<CurrentOrders>(entity =>
            {
                entity.HasKey(op => new
                {
                    op.orderID,
                    op.productID
                });

                // Define relationship between CurrentOrders, Order, and Product entities
                entity.HasOne(op => op.Order)
                .WithMany(a => a.CurrentOrders)
                .HasForeignKey(op => op.orderID)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(op => op.Product)
                .WithMany(a => a.CurrentOrders)
                .HasForeignKey(op => op.productID)
                .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
