using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeJournal.Migrations
{
    /// <inheritdoc />
    public partial class playbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Condition_Playbooks_PlaybookId",
                table: "Condition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Condition",
                table: "Condition");

            migrationBuilder.RenameTable(
                name: "Condition",
                newName: "Conditions");

            migrationBuilder.RenameIndex(
                name: "IX_Condition_PlaybookId",
                table: "Conditions",
                newName: "IX_Conditions_PlaybookId");

            migrationBuilder.AddColumn<string>(
                name: "PlaybookTitle",
                table: "Playbooks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conditions",
                table: "Conditions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conditions_Playbooks_PlaybookId",
                table: "Conditions",
                column: "PlaybookId",
                principalTable: "Playbooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conditions_Playbooks_PlaybookId",
                table: "Conditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conditions",
                table: "Conditions");

            migrationBuilder.DropColumn(
                name: "PlaybookTitle",
                table: "Playbooks");

            migrationBuilder.RenameTable(
                name: "Conditions",
                newName: "Condition");

            migrationBuilder.RenameIndex(
                name: "IX_Conditions_PlaybookId",
                table: "Condition",
                newName: "IX_Condition_PlaybookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Condition",
                table: "Condition",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Condition_Playbooks_PlaybookId",
                table: "Condition",
                column: "PlaybookId",
                principalTable: "Playbooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
