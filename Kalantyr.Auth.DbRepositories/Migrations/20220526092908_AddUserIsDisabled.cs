using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kalantyr.Auth.DbRepositories.Migrations
{
    public partial class AddUserIsDisabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "Users");
        }
    }
}
