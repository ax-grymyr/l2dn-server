using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace L2Dn.GameServer.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountRefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastIpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Crests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<byte[]>(type: "bytea", maxLength: 2176, nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Allys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    CrestId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Allys_Crests_CrestId",
                        column: x => x.CrestId,
                        principalTable: "Crests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    AccessLevel = table.Column<int>(type: "integer", nullable: false),
                    SlotIndex = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    Class = table.Column<short>(type: "smallint", nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false),
                    ExpBeforeDeath = table.Column<long>(type: "bigint", nullable: false),
                    Sp = table.Column<long>(type: "bigint", nullable: false),
                    MaxHp = table.Column<int>(type: "integer", nullable: false),
                    CurrentHp = table.Column<int>(type: "integer", nullable: false),
                    MaxCp = table.Column<int>(type: "integer", nullable: false),
                    CurrentCp = table.Column<int>(type: "integer", nullable: false),
                    MaxMp = table.Column<int>(type: "integer", nullable: false),
                    CurrentMp = table.Column<int>(type: "integer", nullable: false),
                    VitalityPoints = table.Column<int>(type: "integer", nullable: false),
                    PcCafePoints = table.Column<int>(type: "integer", nullable: false),
                    Face = table.Column<byte>(type: "smallint", nullable: false),
                    HairStyle = table.Column<byte>(type: "smallint", nullable: false),
                    HairColor = table.Column<byte>(type: "smallint", nullable: false),
                    Sex = table.Column<byte>(type: "smallint", nullable: false),
                    TransformId = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    NameColor = table.Column<int>(type: "integer", nullable: false),
                    TitleColor = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Z = table.Column<int>(type: "integer", nullable: false),
                    Heading = table.Column<int>(type: "integer", nullable: false),
                    Reputation = table.Column<int>(type: "integer", nullable: false),
                    Fame = table.Column<int>(type: "integer", nullable: false),
                    RaidBossPoints = table.Column<int>(type: "integer", nullable: false),
                    PvpKills = table.Column<int>(type: "integer", nullable: false),
                    PkKills = table.Column<int>(type: "integer", nullable: false),
                    Kills = table.Column<int>(type: "integer", nullable: false),
                    Deaths = table.Column<int>(type: "integer", nullable: false),
                    ClanId = table.Column<int>(type: "integer", nullable: true),
                    ClanPrivileges = table.Column<int>(type: "integer", nullable: false),
                    SubPledge = table.Column<int>(type: "integer", nullable: false),
                    SponsorId = table.Column<int>(type: "integer", nullable: true),
                    LevelJoinedAcademy = table.Column<byte>(type: "smallint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OnlineTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClanCreateExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClanJoinExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Language = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_AccountRefs_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Characters_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    Reputation = table.Column<int>(type: "integer", nullable: false),
                    Castle = table.Column<int>(type: "integer", nullable: true),
                    AllyId = table.Column<int>(type: "integer", nullable: true),
                    LeaderId = table.Column<int>(type: "integer", nullable: false),
                    CrestId = table.Column<int>(type: "integer", nullable: true),
                    LargeCrestId = table.Column<int>(type: "integer", nullable: true),
                    AllyPenaltyExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AllyPenaltyExpireType = table.Column<byte>(type: "smallint", nullable: false),
                    CharPenaltyExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clans_Allys_AllyId",
                        column: x => x.AllyId,
                        principalTable: "Allys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Clans_Characters_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clans_Crests_CrestId",
                        column: x => x.CrestId,
                        principalTable: "Crests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Clans_Crests_LargeCrestId",
                        column: x => x.LargeCrestId,
                        principalTable: "Crests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRefs_Login",
                table: "AccountRefs",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Allys_CrestId",
                table: "Allys",
                column: "CrestId");

            migrationBuilder.CreateIndex(
                name: "IX_Allys_Name",
                table: "Allys",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AccountId",
                table: "Characters",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_ClanId",
                table: "Characters",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Created",
                table: "Characters",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Name",
                table: "Characters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_SponsorId",
                table: "Characters",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clans_AllyId",
                table: "Clans",
                column: "AllyId");

            migrationBuilder.CreateIndex(
                name: "IX_Clans_CrestId",
                table: "Clans",
                column: "CrestId");

            migrationBuilder.CreateIndex(
                name: "IX_Clans_LargeCrestId",
                table: "Clans",
                column: "LargeCrestId");

            migrationBuilder.CreateIndex(
                name: "IX_Clans_LeaderId",
                table: "Clans",
                column: "LeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clans_Name",
                table: "Clans",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Clans_ClanId",
                table: "Characters",
                column: "ClanId",
                principalTable: "Clans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allys_Crests_CrestId",
                table: "Allys");

            migrationBuilder.DropForeignKey(
                name: "FK_Clans_Crests_CrestId",
                table: "Clans");

            migrationBuilder.DropForeignKey(
                name: "FK_Clans_Crests_LargeCrestId",
                table: "Clans");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_AccountRefs_AccountId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Clans_ClanId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "Crests");

            migrationBuilder.DropTable(
                name: "AccountRefs");

            migrationBuilder.DropTable(
                name: "Clans");

            migrationBuilder.DropTable(
                name: "Allys");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
