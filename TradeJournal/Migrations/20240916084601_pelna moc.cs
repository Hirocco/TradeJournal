using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeJournal.Migrations
{
    /// <inheritdoc />
    public partial class pelnamoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Users_UserId",
                table: "Trades");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Trades",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Journals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)");

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Users_UserId",
                table: "Trades",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Users_UserId",
                table: "Trades");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Trades",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Journals",
                type: "nvarchar(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Users_UserId",
                table: "Trades",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
