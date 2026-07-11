using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentManager.Migrations
{
    /// <inheritdoc />
    public partial class AddSendMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendMail",
                table: "AppointmentStatuses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendMail",
                table: "AppointmentStatuses");
        }
    }
}
