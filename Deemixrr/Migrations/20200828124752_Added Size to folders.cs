using Microsoft.EntityFrameworkCore.Migrations;

namespace Deemixrr.Migrations
{
    public partial class AddedSizetofolders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Folders",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Folders");
        }
    }
}
