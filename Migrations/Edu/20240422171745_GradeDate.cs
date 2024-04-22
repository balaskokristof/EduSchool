using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSchool.Migrations.Edu
{
    /// <inheritdoc />
    public partial class GradeDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GradeDate",
                table: "Grades",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GradeDate",
                table: "Grades");
        }
    }
}
