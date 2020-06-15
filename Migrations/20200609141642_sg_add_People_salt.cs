using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretGarden.Migrations
{
    public partial class sg_add_People_salt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "sg_people",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "sg_people");
        }
    }
}
