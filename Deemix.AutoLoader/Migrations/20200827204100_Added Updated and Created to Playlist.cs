using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Deemix.AutoLoader.Migrations
{
    public partial class AddedUpdatedandCreatedtoPlaylist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Playlists",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "NumberOfTracks",
                table: "Playlists",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Playlists",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "NumberOfTracks",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Playlists");
        }
    }
}
