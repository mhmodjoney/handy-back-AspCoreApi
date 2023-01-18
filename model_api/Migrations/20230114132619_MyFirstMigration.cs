using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace model_api.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    state = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    state = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AuthCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExDate = table.Column<DateTime>(nullable: false),
                    code = table.Column<string>(nullable: true),
                    Customer_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthCodes_Customer_Customer_ID",
                        column: x => x.Customer_ID,
                        principalSchema: "dbo",
                        principalTable: "Customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customerlogs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    state = table.Column<string>(nullable: true),
                    Customer_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customerlogs", x => x.id);
                    table.ForeignKey(
                        name: "FK_Customerlogs_Customer_Customer_ID",
                        column: x => x.Customer_ID,
                        principalSchema: "dbo",
                        principalTable: "Customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pyament_infos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    product_id = table.Column<int>(nullable: true),
                    type = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    amount = table.Column<long>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    state = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Customer_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pyament_infos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pyament_infos_Customer_Customer_ID",
                        column: x => x.Customer_ID,
                        principalSchema: "dbo",
                        principalTable: "Customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthCodes_Customer_ID",
                table: "AuthCodes",
                column: "Customer_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Customerlogs_Customer_ID",
                table: "Customerlogs",
                column: "Customer_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pyament_infos_Customer_ID",
                table: "Pyament_infos",
                column: "Customer_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "AuthCodes");

            migrationBuilder.DropTable(
                name: "Customerlogs");

            migrationBuilder.DropTable(
                name: "Pyament_infos");

            migrationBuilder.DropTable(
                name: "Customer",
                schema: "dbo");
        }
    }
}
