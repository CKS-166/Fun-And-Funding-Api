using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Remove_DigitalKey_Requirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_DigitalKey_DigitalKeyID",
                table: "OrderDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "DigitalKeyID",
                table: "OrderDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_DigitalKey_DigitalKeyID",
                table: "OrderDetail",
                column: "DigitalKeyID",
                principalTable: "DigitalKey",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_DigitalKey_DigitalKeyID",
                table: "OrderDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "DigitalKeyID",
                table: "OrderDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_DigitalKey_DigitalKeyID",
                table: "OrderDetail",
                column: "DigitalKeyID",
                principalTable: "DigitalKey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
