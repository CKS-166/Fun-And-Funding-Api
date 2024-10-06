using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class final1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRequirementFiles_ProjectMilestoneRequirements_ProjectRequirementId",
                table: "ProjectRequirementFiles");

            migrationBuilder.RenameColumn(
                name: "ProjectRequirementId",
                table: "ProjectRequirementFiles",
                newName: "ProjectMilestoneRequirementId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectRequirementFiles_ProjectRequirementId",
                table: "ProjectRequirementFiles",
                newName: "IX_ProjectRequirementFiles_ProjectMilestoneRequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRequirementFiles_ProjectMilestoneRequirements_ProjectMilestoneRequirementId",
                table: "ProjectRequirementFiles",
                column: "ProjectMilestoneRequirementId",
                principalTable: "ProjectMilestoneRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRequirementFiles_ProjectMilestoneRequirements_ProjectMilestoneRequirementId",
                table: "ProjectRequirementFiles");

            migrationBuilder.RenameColumn(
                name: "ProjectMilestoneRequirementId",
                table: "ProjectRequirementFiles",
                newName: "ProjectRequirementId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectRequirementFiles_ProjectMilestoneRequirementId",
                table: "ProjectRequirementFiles",
                newName: "IX_ProjectRequirementFiles_ProjectRequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRequirementFiles_ProjectMilestoneRequirements_ProjectRequirementId",
                table: "ProjectRequirementFiles",
                column: "ProjectRequirementId",
                principalTable: "ProjectMilestoneRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
