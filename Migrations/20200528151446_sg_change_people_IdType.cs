using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretGarden.Migrations
{
    public partial class sg_change_people_IdType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdType",
                table: "sg_people",
                type: "enum('身份证')",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('无','身份证','港澳台通行证')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdType",
                table: "sg_people",
                type: "enum('无','身份证','港澳台通行证')",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('身份证')");
        }
    }
}
