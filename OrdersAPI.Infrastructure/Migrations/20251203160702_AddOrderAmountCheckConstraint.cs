using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdersAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAmountCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Order_Amount_MinValue",
                table: "Orders",
                sql: "Amount >= 0.01");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Order_Amount_MinValue",
                table: "Orders");
        }
    }
}
