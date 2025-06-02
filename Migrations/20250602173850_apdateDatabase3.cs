using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomAPI.Migrations
{
    /// <inheritdoc />
    public partial class apdateDatabase3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "dateSubmitted",
                table: "ApplicationHistory",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dateSubmitted",
                table: "ApplicationHistory");
        }
    }
}
