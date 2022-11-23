using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Infrastructure.Migrations
{
    public partial class SplitCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Currencies_Banks_BankId",
                table: "Currencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Currencies_Departments_DepartmentId",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_BankId",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_DepartmentId",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "Buy",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "Sell",
                table: "Currencies");

            migrationBuilder.CreateTable(
                name: "CurrencyExchange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Buy = table.Column<decimal>(type: "decimal(8,5)", precision: 8, scale: 5, nullable: false),
                    Sell = table.Column<decimal>(type: "decimal(8,5)", precision: 8, scale: 5, nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchange_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyExchange_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyExchange_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchange_BankId",
                table: "CurrencyExchange",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchange_CurrencyId",
                table: "CurrencyExchange",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchange_DepartmentId",
                table: "CurrencyExchange",
                column: "DepartmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchange");

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "Currencies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Buy",
                table: "Currencies",
                type: "decimal(8,5)",
                precision: 8,
                scale: 5,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Currencies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Sell",
                table: "Currencies",
                type: "decimal(8,5)",
                precision: 8,
                scale: 5,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_BankId",
                table: "Currencies",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_DepartmentId",
                table: "Currencies",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Banks_BankId",
                table: "Currencies",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Departments_DepartmentId",
                table: "Currencies",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
