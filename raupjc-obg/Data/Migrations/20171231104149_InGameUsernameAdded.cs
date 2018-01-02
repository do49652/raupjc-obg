using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace raupjc_obg.Data.Migrations
{
    public partial class InGameUsernameAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InGameName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InGameName",
                table: "AspNetUsers");
        }
    }
}
