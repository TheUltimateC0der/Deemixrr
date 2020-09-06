using Microsoft.EntityFrameworkCore.Migrations;

namespace Deemix.AutoLoader.Migrations
{
    public partial class AddedNumberOfTracksandNumberOfAlbumstoArtist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "NumberOfAlbums",
                table: "Artists",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "NumberOfTracks",
                table: "Artists",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfAlbums",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "NumberOfTracks",
                table: "Artists");
        }
    }
}
