using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DigitalKey_MarketingProject_MarketingProjectId",
                table: "DigitalKey");

            migrationBuilder.DropTable(
                name: "GamePlatformMarketingProject");

            migrationBuilder.DropTable(
                name: "MarketingFile");

            migrationBuilder.DropTable(
                name: "GamePlatform");

            migrationBuilder.DropTable(
                name: "MarketingProject");

            migrationBuilder.CreateTable(
                name: "MarketplaceProject",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Introduction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FundingProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceProject_FundingProject_FundingProjectId",
                        column: x => x.FundingProjectId,
                        principalTable: "FundingProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filetype = table.Column<int>(type: "int", nullable: false),
                    MarketingProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MarketplaceProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceFile_MarketplaceProject_MarketplaceProjectId",
                        column: x => x.MarketplaceProjectId,
                        principalTable: "MarketplaceProject",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceFile_MarketplaceProjectId",
                table: "MarketplaceFile",
                column: "MarketplaceProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceProject_FundingProjectId",
                table: "MarketplaceProject",
                column: "FundingProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DigitalKey_MarketplaceProject_MarketingProjectId",
                table: "DigitalKey",
                column: "MarketingProjectId",
                principalTable: "MarketplaceProject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DigitalKey_MarketplaceProject_MarketingProjectId",
                table: "DigitalKey");

            migrationBuilder.DropTable(
                name: "MarketplaceFile");

            migrationBuilder.DropTable(
                name: "MarketplaceProject");

            migrationBuilder.CreateTable(
                name: "GamePlatform",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PlatformName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlatform", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingProject",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FundingProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Introduction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingProject_FundingProject_FundingProjectId",
                        column: x => x.FundingProjectId,
                        principalTable: "FundingProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePlatformMarketingProject",
                columns: table => new
                {
                    GamePlatformsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarketingProjectsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlatformMarketingProject", x => new { x.GamePlatformsId, x.MarketingProjectsId });
                    table.ForeignKey(
                        name: "FK_GamePlatformMarketingProject_GamePlatform_GamePlatformsId",
                        column: x => x.GamePlatformsId,
                        principalTable: "GamePlatform",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePlatformMarketingProject_MarketingProject_MarketingProjectsId",
                        column: x => x.MarketingProjectsId,
                        principalTable: "MarketingProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketingFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Filetype = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MarketingProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingFile_MarketingProject_MarketingProjectId",
                        column: x => x.MarketingProjectId,
                        principalTable: "MarketingProject",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GamePlatformMarketingProject_MarketingProjectsId",
                table: "GamePlatformMarketingProject",
                column: "MarketingProjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingFile_MarketingProjectId",
                table: "MarketingFile",
                column: "MarketingProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingProject_FundingProjectId",
                table: "MarketingProject",
                column: "FundingProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DigitalKey_MarketingProject_MarketingProjectId",
                table: "DigitalKey",
                column: "MarketingProjectId",
                principalTable: "MarketingProject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
