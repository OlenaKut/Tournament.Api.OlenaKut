using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tournament.Data.Migrations
{
    /// <inheritdoc />
    public partial class tournament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_TournamentDetails_TournamentDetailsId",
                table: "Game");

            migrationBuilder.RenameColumn(
                name: "TournamentDetailsId",
                table: "Game",
                newName: "TournamentDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Game_TournamentDetailsId",
                table: "Game",
                newName: "IX_Game_TournamentDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_TournamentDetails_TournamentDetailId",
                table: "Game",
                column: "TournamentDetailId",
                principalTable: "TournamentDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_TournamentDetails_TournamentDetailId",
                table: "Game");

            migrationBuilder.RenameColumn(
                name: "TournamentDetailId",
                table: "Game",
                newName: "TournamentDetailsId");

            migrationBuilder.RenameIndex(
                name: "IX_Game_TournamentDetailId",
                table: "Game",
                newName: "IX_Game_TournamentDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_TournamentDetails_TournamentDetailsId",
                table: "Game",
                column: "TournamentDetailsId",
                principalTable: "TournamentDetails",
                principalColumn: "Id");
        }
    }
}
