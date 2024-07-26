using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingCart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_User_PaymentID_as_Null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "strip_CustomerID",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "strip_CustomerID",
                table: "User");
        }
    }
}
