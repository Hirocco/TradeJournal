using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeJournal.Migrations
{
    /// <inheritdoc />
    public partial class ImageTrademerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journals_Image_ImageId",
                table: "Journals");

            migrationBuilder.DropIndex(
                name: "IX_Journals_ImageId",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Journals");

            migrationBuilder.AddColumn<int>(
                name: "TradeId",
                table: "Image",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Image_TradeId",
                table: "Image",
                column: "TradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Trades_TradeId",
                table: "Image",
                column: "TradeId",
                principalTable: "Trades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Trades_TradeId",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_TradeId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "TradeId",
                table: "Image");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Journals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Journals_ImageId",
                table: "Journals",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journals_Image_ImageId",
                table: "Journals",
                column: "ImageId",
                principalTable: "Image",
                principalColumn: "Id");
        }
    }
}
