using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeJournal.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionOpenDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionCloseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SymbolName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionType = table.Column<int>(type: "int", nullable: false),
                    PositionVolume = table.Column<float>(type: "real", nullable: false),
                    EntryPrice = table.Column<float>(type: "real", nullable: false),
                    StopLoss = table.Column<float>(type: "real", nullable: false),
                    TakeProfit = table.Column<float>(type: "real", nullable: false),
                    Comission = table.Column<float>(type: "real", nullable: false),
                    Swap = table.Column<float>(type: "real", nullable: false),
                    TradeOutcome = table.Column<float>(type: "real", nullable: false),
                    PriceChange = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradingAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Server = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingAccounts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "TradingAccounts");
        }
    }
}
