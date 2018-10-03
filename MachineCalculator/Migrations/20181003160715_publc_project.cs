using Microsoft.EntityFrameworkCore.Migrations;

namespace MachineCalculator.Migrations
{
    public partial class publc_project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "ProjectDbSet",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "ProjectDbSet",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "instances",
                table: "ProjectAppDbSet",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "ProjectDbSet");

            migrationBuilder.DropColumn(
                name: "name",
                table: "ProjectDbSet");

            migrationBuilder.DropColumn(
                name: "instances",
                table: "ProjectAppDbSet");
        }
    }
}
