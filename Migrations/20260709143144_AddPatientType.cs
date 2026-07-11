using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentManager.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatientType",
                table: "Patients",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientType",
                table: "Patients");
        }
    }
}
