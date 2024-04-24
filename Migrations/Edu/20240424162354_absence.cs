using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSchool.Migrations.Edu
{
    /// <inheritdoc />
    public partial class absence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Absences",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Absences",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Absences");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Absences",
                newName: "Date");
        }
    }
}
