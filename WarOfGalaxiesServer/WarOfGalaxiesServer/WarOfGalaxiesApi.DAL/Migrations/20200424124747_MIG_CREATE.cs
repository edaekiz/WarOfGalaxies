using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WarOfGalaxiesApi.DAL.Migrations
{
    public partial class MIG_CREATE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_buildings",
                columns: table => new
                {
                    BuildingID = table.Column<int>(nullable: false),
                    BuildingName = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_buildings", x => x.BuildingID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_parameters",
                columns: table => new
                {
                    ParameterID = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    ParameterIntValue = table.Column<int>(nullable: true),
                    ParameterFloatValue = table.Column<double>(nullable: true),
                    ParameterDateTimeValue = table.Column<DateTime>(type: "datetime", nullable: true),
                    ParameterBitValue = table.Column<bool>(nullable: true),
                    ParameterSendToUserValue = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_parameters", x => x.ParameterID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_researches",
                columns: table => new
                {
                    ResearchID = table.Column<int>(nullable: false),
                    ResearchName = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_researches", x => x.ResearchID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user_planet_building_upgs",
                columns: table => new
                {
                    UserPlanetBuildingUpgID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserPlanetID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    BuildingID = table.Column<int>(nullable: false),
                    BuildingLevel = table.Column<int>(nullable: false),
                    BeginDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user_planet_building_upgs", x => x.UserPlanetBuildingUpgID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user_planet_buildings",
                columns: table => new
                {
                    UserPlanetBuildingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserPlanetID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    BuildingID = table.Column<int>(nullable: false),
                    BuildingLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user_planet_buildings", x => x.UserPlanetBuildingID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user_planets",
                columns: table => new
                {
                    UserPlanetID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    PlanetCordinate = table.Column<string>(maxLength: 10, nullable: false),
                    PlanetType = table.Column<int>(nullable: false),
                    PlanetName = table.Column<string>(maxLength: 18, nullable: false),
                    Metal = table.Column<double>(nullable: false),
                    Crystal = table.Column<double>(nullable: false),
                    Boron = table.Column<double>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user_planets", x => x.UserPlanetID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user_research_upgs",
                columns: table => new
                {
                    UserResearchUpgID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    ResearchID = table.Column<int>(nullable: false),
                    ResearchTargetLevel = table.Column<int>(nullable: false),
                    BeginDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user_research_upgs", x => x.UserResearchUpgID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user_researches",
                columns: table => new
                {
                    UserResearchID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    ResearchID = table.Column<int>(nullable: false),
                    ResearchLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user_researches", x => x.UserResearchID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_users",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(maxLength: 16, nullable: false),
                    UserToken = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    IsBanned = table.Column<bool>(nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    GoogleToken = table.Column<string>(maxLength: 50, nullable: true),
                    IosToken = table.Column<string>(maxLength: 50, nullable: true),
                    UserLanguage = table.Column<string>(maxLength: 2, nullable: false, defaultValueSql: "(N'en')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_users", x => x.UserID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_planet_building_upgs_UserID",
                table: "tbl_user_planet_building_upgs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_planet_building_upgs",
                table: "tbl_user_planet_building_upgs",
                column: "UserPlanetID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_planet_buildings",
                table: "tbl_user_planet_buildings",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_planet_buildings",
                table: "tbl_user_planet_buildings",
                columns: new[] { "UserPlanetID", "BuildingID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_planets",
                table: "tbl_user_planets",
                column: "PlanetCordinate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_research_upgs_UserID",
                table: "tbl_user_research_upgs",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_research_upgs_UserID_ResearchID",
                table: "tbl_user_research_upgs",
                columns: new[] { "UserID", "ResearchID" });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_researches_UserID_ResearchID",
                table: "tbl_user_researches",
                columns: new[] { "UserID", "ResearchID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_buildings");

            migrationBuilder.DropTable(
                name: "tbl_parameters");

            migrationBuilder.DropTable(
                name: "tbl_researches");

            migrationBuilder.DropTable(
                name: "tbl_user_planet_building_upgs");

            migrationBuilder.DropTable(
                name: "tbl_user_planet_buildings");

            migrationBuilder.DropTable(
                name: "tbl_user_planets");

            migrationBuilder.DropTable(
                name: "tbl_user_research_upgs");

            migrationBuilder.DropTable(
                name: "tbl_user_researches");

            migrationBuilder.DropTable(
                name: "tbl_users");
        }
    }
}
