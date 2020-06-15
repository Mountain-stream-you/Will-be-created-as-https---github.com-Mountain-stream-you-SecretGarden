using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretGarden.Migrations
{
    public partial class sg_add_ReleaseInformation_Email : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "sg_releaseInformation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "sg_releaseInformation");
        }
    }
}
