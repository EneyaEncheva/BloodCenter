using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCenter.Migrations
{
    /// <inheritdoc />
    public partial class BloodDonor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodDonors_AspNetUsers_ApplicationUserId",
                table: "BloodDonors");

            migrationBuilder.DropIndex(
                name: "IX_BloodDonors_ApplicationUserId",
                table: "BloodDonors");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "BloodDonors");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BloodDonors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonors_UserId",
                table: "BloodDonors",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodDonors_AspNetUsers_UserId",
                table: "BloodDonors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodDonors_AspNetUsers_UserId",
                table: "BloodDonors");

            migrationBuilder.DropIndex(
                name: "IX_BloodDonors_UserId",
                table: "BloodDonors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BloodDonors");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "BloodDonors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonors_ApplicationUserId",
                table: "BloodDonors",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodDonors_AspNetUsers_ApplicationUserId",
                table: "BloodDonors",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
