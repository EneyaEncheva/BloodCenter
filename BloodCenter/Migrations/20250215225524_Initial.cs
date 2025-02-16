using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCenter.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodDonorId = table.Column<int>(type: "int", nullable: false),
                    DonationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonationHistories_BloodDonors_BloodDonorId",
                        column: x => x.BloodDonorId,
                        principalTable: "BloodDonors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonationHistories_BloodDonorId",
                table: "DonationHistories",
                column: "BloodDonorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonationHistories");
        }
    }
}
