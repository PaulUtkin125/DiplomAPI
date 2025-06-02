using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomAPI.Migrations
{
    /// <inheritdoc />
    public partial class apdateDatabase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateEdit = table.Column<DateOnly>(type: "date", nullable: false),
                    timeEdit = table.Column<TimeOnly>(type: "time", nullable: false),
                    UrisidikciiyId = table.Column<int>(type: "int", nullable: false),
                    NameBroker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourseFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsClosing = table.Column<bool>(type: "bit", nullable: false),
                    FullNameOfTheDirector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    INN = table.Column<long>(type: "bigint", nullable: false),
                    KPP = table.Column<long>(type: "bigint", nullable: false),
                    OKTMO = table.Column<long>(type: "bigint", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isAdmitted = table.Column<bool>(type: "bit", nullable: false),
                    TypeOfRequestId = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationHistory", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationHistory");
        }
    }
}
