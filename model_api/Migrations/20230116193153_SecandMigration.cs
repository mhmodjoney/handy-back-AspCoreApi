using Microsoft.EntityFrameworkCore.Migrations;

namespace model_api.Migrations
{
    public partial class SecandMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "payment_method",
                table: "Pyament_infos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment_method",
                table: "Pyament_infos");
        }
    }
}
