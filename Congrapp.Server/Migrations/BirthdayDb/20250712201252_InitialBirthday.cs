using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Congrapp.Server.Migrations.BirthdayDb
{
    /// <inheritdoc />
    public partial class InitialBirthday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BirthdayInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BirthdayDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PersonName = table.Column<string>(type: "TEXT", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BirthdayNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    BirthdayId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayNotes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BirthdayInfos");

            migrationBuilder.DropTable(
                name: "BirthdayNotes");
        }
    }
}
