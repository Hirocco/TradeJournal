using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeJournal.Migrations
{
    /// <inheritdoc />
    public partial class JournalPhotoText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
               name: "Text",
               table: "Journals",
               type: "nvarchar(max)",
               nullable: false);

            migrationBuilder.AlterColumn<byte>(
                name: "ByteStream",
                table: "Journals",
                type:"varbinary(max)",
                nullable: true
                );
    
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
