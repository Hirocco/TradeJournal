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
                name: "Image",
                table: "Journals");

            migrationBuilder.AddColumn<string>(
                name: "ByteStream",
                table: "Journals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ByteStream",
                table: "Journals");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Journals",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
