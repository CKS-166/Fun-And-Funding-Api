using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class final22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMilestoneRequirements_FundingProject_FundingProjectId",
                table: "ProjectMilestoneRequirements");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMilestoneRequirements_FundingProjectId",
                table: "ProjectMilestoneRequirements");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "FundingProjectId",
                table: "ProjectMilestoneRequirements");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "ProjectMilestones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "ProjectMilestoneRequirements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProjectCoupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CouponKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CouponName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    MarketplaceProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCoupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectCoupon_MarketplaceProject_MarketplaceProjectId",
                        column: x => x.MarketplaceProjectId,
                        principalTable: "MarketplaceProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCoupon_MarketplaceProjectId",
                table: "ProjectCoupon",
                column: "MarketplaceProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectCoupon");

            migrationBuilder.DropColumn(
                name: "status",
                table: "ProjectMilestones");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "ProjectMilestoneRequirements");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Requirements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "FundingProjectId",
                table: "ProjectMilestoneRequirements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestoneRequirements_FundingProjectId",
                table: "ProjectMilestoneRequirements",
                column: "FundingProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMilestoneRequirements_FundingProject_FundingProjectId",
                table: "ProjectMilestoneRequirements",
                column: "FundingProjectId",
                principalTable: "FundingProject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
