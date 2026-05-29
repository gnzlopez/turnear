using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentManager.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveProfessionalSpecialty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Specialties",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Professionals",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Professionals");
        }
    }
}
