using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCenter.Migrations
{
    /// <inheritdoc />
    public partial class AvailabililtyRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supplies_BloodGroups_BloodGroupId",
                table: "Supplies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Supplies",
                table: "Supplies");

            migrationBuilder.RenameTable(
                name: "Supplies",
                newName: "Availabilities");

            migrationBuilder.RenameIndex(
                name: "IX_Supplies_BloodGroupId",
                table: "Availabilities",
                newName: "IX_Availabilities_BloodGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Availabilities",
                table: "Availabilities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_BloodGroups_BloodGroupId",
                table: "Availabilities",
                column: "BloodGroupId",
                principalTable: "BloodGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_BloodGroups_BloodGroupId",
                table: "Availabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Availabilities",
                table: "Availabilities");

            migrationBuilder.RenameTable(
                name: "Availabilities",
                newName: "Supplies");

            migrationBuilder.RenameIndex(
                name: "IX_Availabilities_BloodGroupId",
                table: "Supplies",
                newName: "IX_Supplies_BloodGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Supplies",
                table: "Supplies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Supplies_BloodGroups_BloodGroupId",
                table: "Supplies",
                column: "BloodGroupId",
                principalTable: "BloodGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
