using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCenter.Migrations
{
    /// <inheritdoc />
    public partial class FixingRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_RequestedById",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_BloodGroups_BloodGroupId",
                table: "Requests");

            migrationBuilder.AlterColumn<string>(
                name: "RequestedById",
                table: "Requests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "BloodGroupId",
                table: "Requests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_RequestedById",
                table: "Requests",
                column: "RequestedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_BloodGroups_BloodGroupId",
                table: "Requests",
                column: "BloodGroupId",
                principalTable: "BloodGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_RequestedById",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_BloodGroups_BloodGroupId",
                table: "Requests");

            migrationBuilder.AlterColumn<string>(
                name: "RequestedById",
                table: "Requests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

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
                name: "FK_Requests_AspNetUsers_RequestedById",
                table: "Requests",
                column: "RequestedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_BloodGroups_BloodGroupId",
                table: "Requests",
                column: "BloodGroupId",
                principalTable: "BloodGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
