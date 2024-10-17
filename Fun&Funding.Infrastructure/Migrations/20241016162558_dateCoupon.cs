using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dateCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredDate",
                table: "ProjectCoupon",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                table: "ProjectCoupon");
        }
    }
}
