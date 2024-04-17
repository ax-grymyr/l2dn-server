using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace L2Dn.AuthServer.Db.Migrations
{
    /// <inheritdoc />
    public partial class CharacterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountCharacterData_GameServers_ServerId",
                table: "AccountCharacterData");

            migrationBuilder.DropIndex(
                name: "IX_AccountCharacterData_ServerId",
                table: "AccountCharacterData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AccountCharacterData_ServerId",
                table: "AccountCharacterData",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountCharacterData_GameServers_ServerId",
                table: "AccountCharacterData",
                column: "ServerId",
                principalTable: "GameServers",
                principalColumn: "ServerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
