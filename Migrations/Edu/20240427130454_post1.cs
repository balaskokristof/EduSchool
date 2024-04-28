using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSchool.Migrations.Edu
{
    /// <inheritdoc />
    public partial class post1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseID1",
                table: "CoursePost",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoursePost_CourseID1",
                table: "CoursePost",
                column: "CourseID1");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePost_Courses_CourseID1",
                table: "CoursePost",
                column: "CourseID1",
                principalTable: "Courses",
                principalColumn: "CourseID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoursePost_Courses_CourseID1",
                table: "CoursePost");

            migrationBuilder.DropIndex(
                name: "IX_CoursePost_CourseID1",
                table: "CoursePost");

            migrationBuilder.DropColumn(
                name: "CourseID1",
                table: "CoursePost");
        }
    }
}
