using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretGarden.Migrations
{
    public partial class add_sg_releaseInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sg_releaseInformation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "DateTime", nullable: false),
                    EditTime = table.Column<DateTime>(type: "DateTime", nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    PeopleId = table.Column<int>(nullable: false),
                    Convention = table.Column<DateTime>(type: "DateTime", maxLength: 20, nullable: false),
                    Place = table.Column<string>(nullable: true),
                    FailureTime = table.Column<DateTime>(type: "DateTime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sg_releaseInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sg_releaseInformation_sg_people_PeopleId",
                        column: x => x.PeopleId,
                        principalTable: "sg_people",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sg_releaseInformation_PeopleId",
                table: "sg_releaseInformation",
                column: "PeopleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sg_releaseInformation");
        }
    }
}
