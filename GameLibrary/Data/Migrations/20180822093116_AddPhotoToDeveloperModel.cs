using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GameLibrary.Data.Migrations
{
    public partial class AddPhotoToDeveloperModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoId",
                table: "Developers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Developers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Developers");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Developers");
        }
    }
}
