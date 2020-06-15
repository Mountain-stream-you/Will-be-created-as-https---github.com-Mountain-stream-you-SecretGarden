using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretGarden.Migrations
{
    public partial class add_sg_people : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sg_people",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "DateTime", nullable: false),
                    EditTime = table.Column<DateTime>(type: "DateTime", nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Sex = table.Column<string>(type: "enum('男','女')", nullable: false),
                    Age = table.Column<string>(nullable: true),
                    IdType = table.Column<string>(type: "enum('无','身份证','港澳台通行证')", nullable: false),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    Email = table.Column<string>(nullable: true),
                    BirthDay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sg_people", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sg_people");
        }
    }
}
