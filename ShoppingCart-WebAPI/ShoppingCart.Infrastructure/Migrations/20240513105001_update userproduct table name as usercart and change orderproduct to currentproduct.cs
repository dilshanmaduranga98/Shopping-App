using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingCart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateuserproducttablenameasusercartandchangeorderproducttocurrentproduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProducts");

            migrationBuilder.DropTable(
                name: "UserProduct");

            migrationBuilder.CreateTable(
                name: "CurrentOrders",
                columns: table => new
                {
                    orderID = table.Column<int>(type: "int", nullable: false),
                    productID = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<double>(type: "float", nullable: false),
                    orderQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentOrders", x => new { x.orderID, x.productID });
                    table.ForeignKey(
                        name: "FK_CurrentOrders_Order_orderID",
                        column: x => x.orderID,
                        principalTable: "Order",
                        principalColumn: "orderID");
                    table.ForeignKey(
                        name: "FK_CurrentOrders_Product_productID",
                        column: x => x.productID,
                        principalTable: "Product",
                        principalColumn: "productID");
                });

            migrationBuilder.CreateTable(
                name: "UserCart",
                columns: table => new
                {
                    userID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    productID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCart", x => new { x.userID, x.productID });
                    table.ForeignKey(
                        name: "FK_UserCart_Product_productID",
                        column: x => x.productID,
                        principalTable: "Product",
                        principalColumn: "productID");
                    table.ForeignKey(
                        name: "FK_UserCart_User_userID",
                        column: x => x.userID,
                        principalTable: "User",
                        principalColumn: "userID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrentOrders_productID",
                table: "CurrentOrders",
                column: "productID");

            migrationBuilder.CreateIndex(
                name: "IX_UserCart_productID",
                table: "UserCart",
                column: "productID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentOrders");

            migrationBuilder.DropTable(
                name: "UserCart");

            migrationBuilder.CreateTable(
                name: "OrderProducts",
                columns: table => new
                {
                    orderID = table.Column<int>(type: "int", nullable: false),
                    productID = table.Column<int>(type: "int", nullable: false),
                    orderQuantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProducts", x => new { x.orderID, x.productID });
                    table.ForeignKey(
                        name: "FK_OrderProducts_Order_orderID",
                        column: x => x.orderID,
                        principalTable: "Order",
                        principalColumn: "orderID");
                    table.ForeignKey(
                        name: "FK_OrderProducts_Product_productID",
                        column: x => x.productID,
                        principalTable: "Product",
                        principalColumn: "productID");
                });

            migrationBuilder.CreateTable(
                name: "UserProduct",
                columns: table => new
                {
                    userID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    productID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProduct", x => new { x.userID, x.productID });
                    table.ForeignKey(
                        name: "FK_UserProduct_Product_productID",
                        column: x => x.productID,
                        principalTable: "Product",
                        principalColumn: "productID");
                    table.ForeignKey(
                        name: "FK_UserProduct_User_userID",
                        column: x => x.userID,
                        principalTable: "User",
                        principalColumn: "userID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_productID",
                table: "OrderProducts",
                column: "productID");

            migrationBuilder.CreateIndex(
                name: "IX_UserProduct_productID",
                table: "UserProduct",
                column: "productID");
        }
    }
}
