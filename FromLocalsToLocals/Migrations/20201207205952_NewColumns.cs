using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FromLocalsToLocals.Migrations
{
    public partial class NewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
               name: "LastClickDate",
               table: "Vendors",
               type: "timestamp without time zone",
               nullable: false,
               defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Popularity",
                table: "Vendors",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastClickDate",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Popularity",
                table: "Vendors");
        }
    }
}
