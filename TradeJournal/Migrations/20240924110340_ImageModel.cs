using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeJournal.Migrations
{
    /// <inheritdoc />
    public partial class ImageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ByteStream",
                table: "Journals");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Journals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Journals_ImageId",
                table: "Journals",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journals_Image_ImageId",
                table: "Journals",
                column: "ImageId",
                principalTable: "Image",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journals_Image_ImageId",
                table: "Journals");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Journals_ImageId",
                table: "Journals");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Journals");

            migrationBuilder.AddColumn<byte[]>(
                name: "ByteStream",
                table: "Journals",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
