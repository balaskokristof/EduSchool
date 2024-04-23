using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSchool.Migrations.Edu
{
    /// <inheritdoc />
    public partial class buggol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Absences_AbsenceTypes_AbsenceTypeID1",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_Absences_AbsenceTypeID1",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "AbsenceTypeID1",
                table: "Absences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AbsenceTypeID1",
                table: "Absences",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Absences_AbsenceTypeID1",
                table: "Absences",
                column: "AbsenceTypeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Absences_AbsenceTypes_AbsenceTypeID1",
                table: "Absences",
                column: "AbsenceTypeID1",
                principalTable: "AbsenceTypes",
                principalColumn: "AbsenceTypeID");
        }
    }
}
