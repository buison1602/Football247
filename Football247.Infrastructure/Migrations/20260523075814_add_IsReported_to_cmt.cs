using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_IsReported_to_cmt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReported",
                table: "Comments",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReported",
                table: "Comments");
        }
    }
}
