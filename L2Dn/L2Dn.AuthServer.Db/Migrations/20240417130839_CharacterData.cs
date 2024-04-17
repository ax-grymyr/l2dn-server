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
            migrationBuilder.CreateTable(
                name: "AccountCharacterData",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    ServerId = table.Column<byte>(type: "smallint", nullable: false),
                    CharacterCount = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCharacterData", x => new { x.AccountId, x.ServerId });
                    table.ForeignKey(
                        name: "FK_AccountCharacterData_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountCharacterData_GameServers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "GameServers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCharacterData_ServerId",
                table: "AccountCharacterData",
                column: "ServerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCharacterData");
        }
    }
}
