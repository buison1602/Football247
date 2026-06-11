using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_notification_message_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReceiveEmailNotifications",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiveInAppNotifications",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiveEmailNotifications",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ReceiveInAppNotifications",
                table: "AspNetUsers");
        }
    }
}
