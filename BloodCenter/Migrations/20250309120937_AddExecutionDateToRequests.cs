using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddExecutionDateToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutionDate",
                table: "Requests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionDate",
                table: "Requests");
        }
    }
}
