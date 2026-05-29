using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentManager.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationToAppointmentAndProfessional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Professionals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Professionals");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Appointments");
        }
    }
}
