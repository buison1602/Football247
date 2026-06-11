using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Football247.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_3_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ExternalId = table.Column<int>(type: "int", nullable: false),
                    UtcDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Matchday = table.Column<int>(type: "int", nullable: false),
                    HomeTeamExternalId = table.Column<int>(type: "int", nullable: false),
                    HomeTeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeTeamShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeTeamCrest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeScore = table.Column<int>(type: "int", nullable: true),
                    AwayTeamExternalId = table.Column<int>(type: "int", nullable: false),
                    AwayTeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwayTeamShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwayTeamCrest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwayScore = table.Column<int>(type: "int", nullable: true),
                    CompetitionCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompetitionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Season = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Standings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CompetitionCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Season = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    TeamExternalId = table.Column<int>(type: "int", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamCrest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayedGames = table.Column<int>(type: "int", nullable: false),
                    Won = table.Column<int>(type: "int", nullable: false),
                    Draw = table.Column<int>(type: "int", nullable: false),
                    Lost = table.Column<int>(type: "int", nullable: false),
                    GoalDifference = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Standings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_ExternalId",
                table: "Matches",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Listing_Optimized",
                table: "Matches",
                columns: new[] { "CompetitionCode", "Season", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_UtcDate",
                table: "Matches",
                column: "UtcDate");

            migrationBuilder.CreateIndex(
                name: "IX_Standings_Competition_Season_Position",
                table: "Standings",
                columns: new[] { "CompetitionCode", "Season", "Position" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Standings");
        }
    }
}
