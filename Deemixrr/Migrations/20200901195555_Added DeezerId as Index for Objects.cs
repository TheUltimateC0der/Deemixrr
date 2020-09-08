using Microsoft.EntityFrameworkCore.Migrations;

namespace Deemixrr.Migrations
{
    public partial class AddedDeezerIdasIndexforObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Playlists_DeezerId",
                table: "Playlists",
                column: "DeezerId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_DeezerId",
                table: "Genres",
                column: "DeezerId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_DeezerId",
                table: "Artists",
                column: "DeezerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Playlists_DeezerId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Genres_DeezerId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Artists_DeezerId",
                table: "Artists");
        }
    }
}
