using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingCart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcartcreateandupdatedateinusercarttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "cartCreateDate",
                table: "UserCart",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "cartUpdateDate",
                table: "UserCart",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cartCreateDate",
                table: "UserCart");

            migrationBuilder.DropColumn(
                name: "cartUpdateDate",
                table: "UserCart");
        }
    }
}
