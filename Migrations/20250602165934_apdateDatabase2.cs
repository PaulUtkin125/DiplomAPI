using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomAPI.Migrations
{
    /// <inheritdoc />
    public partial class apdateDatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "mainId",
                table: "ApplicationHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mainId",
                table: "ApplicationHistory");
        }
    }
}
