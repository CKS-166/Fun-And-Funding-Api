using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removewalletid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawRequest_Wallet_WalletId",
                table: "WithdrawRequest");

            migrationBuilder.AlterColumn<Guid>(
                name: "WalletId",
                table: "WithdrawRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawRequest_Wallet_WalletId",
                table: "WithdrawRequest",
                column: "WalletId",
                principalTable: "Wallet",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawRequest_Wallet_WalletId",
                table: "WithdrawRequest");

            migrationBuilder.AlterColumn<Guid>(
                name: "WalletId",
                table: "WithdrawRequest",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawRequest_Wallet_WalletId",
                table: "WithdrawRequest",
                column: "WalletId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
