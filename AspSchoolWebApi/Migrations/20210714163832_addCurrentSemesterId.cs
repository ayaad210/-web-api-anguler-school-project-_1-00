using Microsoft.EntityFrameworkCore.Migrations;

namespace AspSchoolWebApi.Migrations
{
    public partial class addCurrentSemesterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentSemesterId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSemesterId",
                table: "Students");
        }
    }
}
