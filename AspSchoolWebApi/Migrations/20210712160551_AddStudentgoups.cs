using Microsoft.EntityFrameworkCore.Migrations;

namespace AspSchoolWebApi.Migrations
{
    public partial class AddStudentgoups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Groups_Groupid",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_Groupid",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Groupid",
                table: "Students");

            migrationBuilder.CreateTable(
                name: "GroupStudent",
                columns: table => new
                {
                    Groupsid = table.Column<int>(type: "int", nullable: false),
                    Studentsid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupStudent", x => new { x.Groupsid, x.Studentsid });
                    table.ForeignKey(
                        name: "FK_GroupStudent_Groups_Groupsid",
                        column: x => x.Groupsid,
                        principalTable: "Groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupStudent_Students_Studentsid",
                        column: x => x.Studentsid,
                        principalTable: "Students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupStudent_Studentsid",
                table: "GroupStudent",
                column: "Studentsid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupStudent");

            migrationBuilder.AddColumn<int>(
                name: "Groupid",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Groupid",
                table: "Students",
                column: "Groupid");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Groups_Groupid",
                table: "Students",
                column: "Groupid",
                principalTable: "Groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
