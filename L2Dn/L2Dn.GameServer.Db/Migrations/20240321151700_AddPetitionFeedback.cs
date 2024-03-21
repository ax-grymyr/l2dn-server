using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace L2Dn.GameServer.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddPetitionFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PetitionFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterName = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    GmName = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    Rate = table.Column<byte>(type: "smallint", nullable: false),
                    Message = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetitionFeedbacks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetitionFeedbacks");
        }
    }
}
