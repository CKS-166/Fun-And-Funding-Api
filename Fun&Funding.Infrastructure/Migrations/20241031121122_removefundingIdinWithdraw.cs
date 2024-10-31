using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removefundingIdinWithdraw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawRequest_FundingProject_FundingProjectId",
                table: "WithdrawRequest");

            migrationBuilder.DropIndex(
                name: "IX_WithdrawRequest_FundingProjectId",
                table: "WithdrawRequest");

            migrationBuilder.DropColumn(
                name: "FundingProjectId",
                table: "WithdrawRequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FundingProjectId",
                table: "WithdrawRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawRequest_FundingProjectId",
                table: "WithdrawRequest",
                column: "FundingProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawRequest_FundingProject_FundingProjectId",
                table: "WithdrawRequest",
                column: "FundingProjectId",
                principalTable: "FundingProject",
                principalColumn: "Id");
        }
    }
}
