using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace raupjc_obg.Data.Migrations
{
    public partial class ReviewStuffAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<bool>(
                name: "Admin",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GamesPlayed",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GamesWon",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GameModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GameModelId = table.Column<Guid>(nullable: true),
                    MiniEvents = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Private = table.Column<bool>(nullable: false),
                    SetEvents = table.Column<string>(nullable: true),
                    Standalone = table.Column<bool>(nullable: false),
                    StartingMoney = table.Column<float>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameModel_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameModel_GameModel_GameModelId",
                        column: x => x.GameModelId,
                        principalTable: "GameModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Behaviour = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GameId = table.Column<Guid>(nullable: true),
                    HappensOnce = table.Column<bool>(nullable: false),
                    Items = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NextEventId = table.Column<Guid>(nullable: true),
                    Repeat = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventModel_GameModel_GameId",
                        column: x => x.GameId,
                        principalTable: "GameModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventModel_EventModel_NextEventId",
                        column: x => x.NextEventId,
                        principalTable: "EventModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Behaviour = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GameId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemModel_GameModel_GameId",
                        column: x => x.GameId,
                        principalTable: "GameModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventModel_GameId",
                table: "EventModel",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_EventModel_NextEventId",
                table: "EventModel",
                column: "NextEventId");

            migrationBuilder.CreateIndex(
                name: "IX_GameModel_ApplicationUserId",
                table: "GameModel",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameModel_GameModelId",
                table: "GameModel",
                column: "GameModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemModel_GameId",
                table: "ItemModel",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "EventModel");

            migrationBuilder.DropTable(
                name: "ItemModel");

            migrationBuilder.DropTable(
                name: "GameModel");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Admin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GamesPlayed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GamesWon",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
