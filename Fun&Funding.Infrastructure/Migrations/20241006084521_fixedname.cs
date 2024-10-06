using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fun_Funding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixedname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRequirementFiles_ProjectRequirements_ProjectRequirementId",
                table: "ProjectRequirementFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRequirements_FundingProject_FundingProjectId",
                table: "ProjectRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRequirements_ProjectMilestones_ProjectMilestoneId",
                table: "ProjectRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRequirements_Requirements_RequirementId",
                table: "ProjectRequirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectRequirements",
                table: "ProjectRequirements");

            migrationBuilder.RenameTable(
                name: "ProjectRequirements",
                newName: "ProjectMilestoneRequirements");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectRequirements_RequirementId",
                table: "ProjectMilestoneRequirements",
                newName: "IX_ProjectMilestoneRequirements_RequirementId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectRequirements_ProjectMilestoneId",
                table: "ProjectMilestoneRequirements",
                newName: "IX_ProjectMilestoneRequirements_ProjectMilestoneId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectRequirements_FundingProjectId",
                table: "ProjectMilestoneRequirements",
                newName: "IX_ProjectMilestoneRequirements_FundingProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMilestoneRequirements",
                table: "ProjectMilestoneRequirements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMilestoneRequirements_FundingProject_FundingProjectId",
                table: "ProjectMilestoneRequirements",
                column: "FundingProjectId",
                principalTable: "FundingProject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMilestoneRequirements_ProjectMilestones_ProjectMilestoneId",
                table: "ProjectMilestoneRequirements",
                column: "ProjectMilestoneId",
                principalTable: "ProjectMilestones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMilestoneRequirements_Requirements_RequirementId",
                table: "ProjectMilestoneRequirements",
                column: "RequirementId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRequirementFiles_ProjectMilestoneRequirements_ProjectRequirementId",
                table: "ProjectRequirementFiles",
                column: "ProjectRequirementId",
                principalTable: "ProjectMilestoneRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMilestoneRequirements_FundingProject_FundingProjectId",
                table: "ProjectMilestoneRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMilestoneRequirements_ProjectMilestones_ProjectMilestoneId",
                table: "ProjectMilestoneRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMilestoneRequirements_Requirements_RequirementId",
                table: "ProjectMilestoneRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRequirementFiles_ProjectMilestoneRequirements_ProjectRequirementId",
                table: "ProjectRequirementFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMilestoneRequirements",
                table: "ProjectMilestoneRequirements");

            migrationBuilder.RenameTable(
                name: "ProjectMilestoneRequirements",
                newName: "ProjectRequirements");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMilestoneRequirements_RequirementId",
                table: "ProjectRequirements",
                newName: "IX_ProjectRequirements_RequirementId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMilestoneRequirements_ProjectMilestoneId",
                table: "ProjectRequirements",
                newName: "IX_ProjectRequirements_ProjectMilestoneId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMilestoneRequirements_FundingProjectId",
                table: "ProjectRequirements",
                newName: "IX_ProjectRequirements_FundingProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectRequirements",
                table: "ProjectRequirements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRequirementFiles_ProjectRequirements_ProjectRequirementId",
                table: "ProjectRequirementFiles",
                column: "ProjectRequirementId",
                principalTable: "ProjectRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRequirements_FundingProject_FundingProjectId",
                table: "ProjectRequirements",
                column: "FundingProjectId",
                principalTable: "FundingProject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRequirements_ProjectMilestones_ProjectMilestoneId",
                table: "ProjectRequirements",
                column: "ProjectMilestoneId",
                principalTable: "ProjectMilestones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRequirements_Requirements_RequirementId",
                table: "ProjectRequirements",
                column: "RequirementId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
