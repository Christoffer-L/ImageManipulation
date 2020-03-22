using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImageManipulationApi.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserManipulatedImages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    EncryptedImage = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserManipulatedImages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserManipulatedImages");
        }
    }
}
