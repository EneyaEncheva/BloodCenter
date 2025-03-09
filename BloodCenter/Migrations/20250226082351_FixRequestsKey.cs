using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCenter.Migrations
{
    /// <inheritdoc />
    public partial class FixRequestsKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_BloodGroups_BloodGroupId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "BloodGroupsId",
                table: "Requests");

            migrationBuilder.AlterColumn<int>(
                name: "BloodGroupId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_BloodGroups_BloodGroupId",
                table: "Requests",
                column: "BloodGroupId",
                principalTable: "BloodGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_BloodGroups_BloodGroupId",
                table: "Requests");

            migrationBuilder.AlterColumn<int>(
                name: "BloodGroupId",
                table: "Requests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BloodGroupsId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_BloodGroups_BloodGroupId",
                table: "Requests",
                column: "BloodGroupId",
                principalTable: "BloodGroups",
                principalColumn: "Id");
        }
    }
}
