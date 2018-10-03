using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MachineCalculator.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppObjDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(nullable: true),
                    flag = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppObjDbSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDbSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppParameterDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AppId = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    load = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppParameterDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppParameterDbSet_AppObjDbSet_AppId",
                        column: x => x.AppId,
                        principalTable: "AppObjDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAppDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    projectId = table.Column<int>(nullable: false),
                    appId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAppDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAppDbSet_AppObjDbSet_appId",
                        column: x => x.appId,
                        principalTable: "AppObjDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectAppDbSet_ProjectDbSet_projectId",
                        column: x => x.projectId,
                        principalTable: "ProjectDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppParameterDbSet_AppId",
                table: "AppParameterDbSet",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAppDbSet_appId",
                table: "ProjectAppDbSet",
                column: "appId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAppDbSet_projectId",
                table: "ProjectAppDbSet",
                column: "projectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppParameterDbSet");

            migrationBuilder.DropTable(
                name: "ProjectAppDbSet");

            migrationBuilder.DropTable(
                name: "AppObjDbSet");

            migrationBuilder.DropTable(
                name: "ProjectDbSet");
        }
    }
}
