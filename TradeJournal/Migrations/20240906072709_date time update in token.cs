using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeJournal.Migrations
{
    /// <inheritdoc />
    public partial class datetimeupdateintoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenCreatedAt",
                table: "Tokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenCreatedAt",
                table: "Tokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
