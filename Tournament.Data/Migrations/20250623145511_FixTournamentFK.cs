using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tournament.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTournamentFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_TournamentDetails_TournamentDetailId",
                table: "Game");

            migrationBuilder.DropIndex(
                name: "IX_Game_TournamentDetailId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "TournamentDetailId",
                table: "Game");

            migrationBuilder.CreateIndex(
                name: "IX_Game_TournamentId",
                table: "Game",
                column: "TournamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_TournamentDetails_TournamentId",
                table: "Game",
                column: "TournamentId",
                principalTable: "TournamentDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_TournamentDetails_TournamentId",
                table: "Game");

            migrationBuilder.DropIndex(
                name: "IX_Game_TournamentId",
                table: "Game");

            migrationBuilder.AddColumn<int>(
                name: "TournamentDetailId",
                table: "Game",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Game_TournamentDetailId",
                table: "Game",
                column: "TournamentDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_TournamentDetails_TournamentDetailId",
                table: "Game",
                column: "TournamentDetailId",
                principalTable: "TournamentDetails",
                principalColumn: "Id");
        }
    }
}
