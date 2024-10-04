using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeJournal.Migrations
{
    /// <inheritdoc />
    public partial class photo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "byteStraeam",
                table: "Journals");

            migrationBuilder.AddColumn<byte[]>(
                name: "ByteStream",
                table: "Journals",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ByteStream",
                table: "Journals");

            migrationBuilder.AddColumn<string>(
                name: "byteStraeam",
                table: "Journals",
                type: "nvarbinary(max)",
                nullable: true);
        }
    }
}
