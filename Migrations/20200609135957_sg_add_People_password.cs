using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretGarden.Migrations
{
    public partial class sg_add_People_password : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NetName",
                table: "sg_people",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "sg_people",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetName",
                table: "sg_people");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "sg_people");
        }
    }
}
