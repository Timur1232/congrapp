using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace congrapp.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    PersonName = table.Column<string>(type: "TEXT", nullable: false),
                    BirthdayDate = table.Column<DateTime>(type: "TEXT", nullable: false),
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
                    BirthdayId = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthdayNotes_BirthdayInfos_BirthdayId",
                        column: x => x.BirthdayId,
                        principalTable: "BirthdayInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayNotes_BirthdayId",
                table: "BirthdayNotes",
                column: "BirthdayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BirthdayNotes");

            migrationBuilder.DropTable(
                name: "BirthdayInfos");
        }
    }
}
