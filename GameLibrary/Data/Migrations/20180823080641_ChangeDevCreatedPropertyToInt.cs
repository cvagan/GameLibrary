using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GameLibrary.Data.Migrations
{
    public partial class ChangeDevCreatedPropertyToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Created",
                table: "Developers",
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Created",
                table: "Developers",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
