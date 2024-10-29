using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newOrderReq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Requirements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Requirements");
        }
    }
}
