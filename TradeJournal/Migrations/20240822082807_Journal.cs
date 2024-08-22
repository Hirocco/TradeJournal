using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeJournal.Migrations
{
    /// <inheritdoc />
    public partial class Journal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Journals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Journals",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Journals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
