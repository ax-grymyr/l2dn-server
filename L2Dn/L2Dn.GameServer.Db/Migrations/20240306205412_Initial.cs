using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace L2Dn.GameServer.Db.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountCollectionFavorites",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    CollectionId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCollectionFavorites", x => new { x.AccountId, x.CollectionId });
                });

            migrationBuilder.CreateTable(
                name: "AccountCollections",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    CollectionId = table.Column<short>(type: "smallint", nullable: false),
                    Index = table.Column<short>(type: "smallint", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCollections", x => new { x.AccountId, x.CollectionId, x.Index });
                });

            migrationBuilder.CreateTable(
                name: "AccountPremiums",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPremiums", x => x.AccountId);
                });

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
                name: "AccountVariables",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountVariables", x => new { x.AccountId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "AchievementBoxes",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BoxOwned = table.Column<int>(type: "integer", nullable: false),
                    MonsterPoint = table.Column<int>(type: "integer", nullable: false),
                    PvpPoint = table.Column<int>(type: "integer", nullable: false),
                    PendingBox = table.Column<int>(type: "integer", nullable: false),
                    OpenTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BoxStateSlot1 = table.Column<int>(type: "integer", nullable: false),
                    BoxTypeSlot1 = table.Column<int>(type: "integer", nullable: false),
                    BoxStateSlot2 = table.Column<int>(type: "integer", nullable: false),
                    BoxTypeSlot2 = table.Column<int>(type: "integer", nullable: false),
                    BoxStateSlot3 = table.Column<int>(type: "integer", nullable: false),
                    BoxTypeSlot3 = table.Column<int>(type: "integer", nullable: false),
                    BoxStateSlot4 = table.Column<int>(type: "integer", nullable: false),
                    BoxTypeSlot4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementBoxes", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "AirShips",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fuel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirShips", x => x.OwnerId);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    InitialDelay = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Period = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Repeat = table.Column<int>(type: "integer", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotReports",
                columns: table => new
                {
                    BotId = table.Column<int>(type: "integer", nullable: false),
                    ReporterId = table.Column<int>(type: "integer", nullable: false),
                    ReportTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotReports", x => new { x.BotId, x.ReporterId });
                });

            migrationBuilder.CreateTable(
                name: "BufferSchemes",
                columns: table => new
                {
                    ObjectId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Skills = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BufferSchemes", x => new { x.ObjectId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "BuyLists",
                columns: table => new
                {
                    BuyListId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    NextRestockTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyLists", x => new { x.BuyListId, x.ItemId });
                });

            migrationBuilder.CreateTable(
                name: "CastleDoorUpgrades",
                columns: table => new
                {
                    DoorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ratio = table.Column<short>(type: "smallint", nullable: false),
                    CastleId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleDoorUpgrades", x => x.DoorId);
                });

            migrationBuilder.CreateTable(
                name: "CastleFunctions",
                columns: table => new
                {
                    CastleId = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
                    Lease = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleFunctions", x => new { x.CastleId, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "CastleManorProcure",
                columns: table => new
                {
                    CastleId = table.Column<short>(type: "smallint", nullable: false),
                    CropId = table.Column<int>(type: "integer", nullable: false),
                    NextPeriod = table.Column<bool>(type: "boolean", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    StartAmount = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    RewardType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleManorProcure", x => new { x.CastleId, x.CropId, x.NextPeriod });
                });

            migrationBuilder.CreateTable(
                name: "CastleManorProduction",
                columns: table => new
                {
                    CastleId = table.Column<short>(type: "smallint", nullable: false),
                    SeedId = table.Column<int>(type: "integer", nullable: false),
                    NextPeriod = table.Column<bool>(type: "boolean", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    StartAmount = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleManorProduction", x => new { x.CastleId, x.SeedId, x.NextPeriod });
                });

            migrationBuilder.CreateTable(
                name: "Castles",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Side = table.Column<int>(type: "integer", nullable: false),
                    Treasury = table.Column<long>(type: "bigint", nullable: false),
                    SiegeTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegistrationTimeOver = table.Column<bool>(type: "boolean", nullable: false),
                    RegistrationEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShowNpcCrest = table.Column<bool>(type: "boolean", nullable: false),
                    TicketBuyCount = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Castles", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "CastleSiegeClans",
                columns: table => new
                {
                    CastleId = table.Column<byte>(type: "smallint", nullable: false),
                    ClanId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    IsCastleOwner = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeClans", x => new { x.ClanId, x.CastleId });
                });

            migrationBuilder.CreateTable(
                name: "CastleSiegeGuards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CastleId = table.Column<short>(type: "smallint", nullable: false),
                    NpcId = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Z = table.Column<int>(type: "integer", nullable: false),
                    Heading = table.Column<int>(type: "integer", nullable: false),
                    RespawnDelay = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsHired = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeGuards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CastleTrapUpgrades",
                columns: table => new
                {
                    CastleId = table.Column<short>(type: "smallint", nullable: false),
                    TowerIndex = table.Column<short>(type: "smallint", nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleTrapUpgrades", x => new { x.TowerIndex, x.CastleId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterContacts",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterContacts", x => new { x.CharacterId, x.ContactId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterCouples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Character1Id = table.Column<int>(type: "integer", nullable: false),
                    Character2Id = table.Column<int>(type: "integer", nullable: false),
                    Married = table.Column<bool>(type: "boolean", nullable: false),
                    AffianceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WeddingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterCouples", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterDailyRewards",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    RewardId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    LastCompleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterDailyRewards", x => new { x.CharacterId, x.RewardId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterHennaPotens",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    SlotPosition = table.Column<int>(type: "integer", nullable: false),
                    PotenId = table.Column<int>(type: "integer", nullable: false),
                    EnchantLevel = table.Column<int>(type: "integer", nullable: false),
                    EnchantExp = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterHennaPotens", x => new { x.CharacterId, x.SlotPosition });
                });

            migrationBuilder.CreateTable(
                name: "CharacterHennas",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    ClassIndex = table.Column<byte>(type: "smallint", nullable: false),
                    SymbolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterHennas", x => new { x.CharacterId, x.ClassIndex, x.Slot });
                });

            migrationBuilder.CreateTable(
                name: "CharacterInstances",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    InstanceId = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInstances", x => new { x.CharacterId, x.InstanceId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterItemReuses",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    ItemObjectId = table.Column<int>(type: "integer", nullable: false),
                    ReuseDelay = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SysTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterItemReuses", x => new { x.CharacterId, x.ItemId, x.ItemObjectId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterMacros",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Acronym = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    Commands = table.Column<string>(type: "character varying(1255)", maxLength: 1255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMacros", x => new { x.CharacterId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "CharacterMentees",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    MentorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMentees", x => new { x.CharacterId, x.MentorId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterPremiumItems",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    ItemNumber = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    ItemCount = table.Column<long>(type: "bigint", nullable: false),
                    ItemSender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterPremiumItems", x => new { x.CharacterId, x.ItemNumber, x.ItemId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterPurges",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<short>(type: "smallint", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Keys = table.Column<int>(type: "integer", nullable: false),
                    RemainingKeys = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterPurges", x => new { x.CharacterId, x.Category });
                });

            migrationBuilder.CreateTable(
                name: "CharacterQuests",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Variable = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterQuests", x => new { x.CharacterId, x.Name, x.Variable });
                });

            migrationBuilder.CreateTable(
                name: "CharacterRandomCrafts",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullPoints = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    IsSayhaRoll = table.Column<bool>(type: "boolean", nullable: false),
                    Item1Id = table.Column<int>(type: "integer", nullable: false),
                    Item1Count = table.Column<long>(type: "bigint", nullable: false),
                    Item1Locked = table.Column<bool>(type: "boolean", nullable: false),
                    Item1LockLeft = table.Column<int>(type: "integer", nullable: false),
                    Item2Id = table.Column<int>(type: "integer", nullable: false),
                    Item2Count = table.Column<long>(type: "bigint", nullable: false),
                    Item2Locked = table.Column<bool>(type: "boolean", nullable: false),
                    Item2LockLeft = table.Column<int>(type: "integer", nullable: false),
                    Item3Id = table.Column<int>(type: "integer", nullable: false),
                    Item3Count = table.Column<long>(type: "bigint", nullable: false),
                    Item3Locked = table.Column<bool>(type: "boolean", nullable: false),
                    Item3LockLeft = table.Column<int>(type: "integer", nullable: false),
                    Item4Id = table.Column<int>(type: "integer", nullable: false),
                    Item4Count = table.Column<long>(type: "bigint", nullable: false),
                    Item4Locked = table.Column<bool>(type: "boolean", nullable: false),
                    Item4LockLeft = table.Column<int>(type: "integer", nullable: false),
                    Item5Id = table.Column<int>(type: "integer", nullable: false),
                    Item5Count = table.Column<long>(type: "bigint", nullable: false),
                    Item5Locked = table.Column<bool>(type: "boolean", nullable: false),
                    Item5LockLeft = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterRandomCrafts", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "CharacterRankingHistory",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Ranking = table.Column<int>(type: "integer", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterRankingHistory", x => new { x.CharacterId, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "CharacterRecipeBooks",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ClassIndex = table.Column<short>(type: "smallint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterRecipeBooks", x => new { x.Id, x.CharacterId, x.ClassIndex });
                });

            migrationBuilder.CreateTable(
                name: "CharacterRecipeShopLists",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    RecipeId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    Index = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterRecipeShopLists", x => new { x.CharacterId, x.RecipeId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterRecoBonuses",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecHave = table.Column<short>(type: "smallint", nullable: false),
                    RecLeft = table.Column<short>(type: "smallint", nullable: false),
                    TimeLeft = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterRecoBonuses", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "CharacterRevenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    KillerName = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    KillerClan = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    KillerLevel = table.Column<int>(type: "integer", nullable: false),
                    KillerClass = table.Column<short>(type: "smallint", nullable: false),
                    VictimName = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    VictimClan = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    VictimLevel = table.Column<int>(type: "integer", nullable: false),
                    VictimClass = table.Column<short>(type: "smallint", nullable: false),
                    Shared = table.Column<bool>(type: "boolean", nullable: false),
                    ShowLocationRemaining = table.Column<int>(type: "integer", nullable: false),
                    TeleportRemaining = table.Column<int>(type: "integer", nullable: false),
                    SharedTeleportRemaining = table.Column<int>(type: "integer", nullable: false),
                    KillTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShareTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterRevenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterShortCuts",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Slot = table.Column<byte>(type: "smallint", nullable: false),
                    Page = table.Column<byte>(type: "smallint", nullable: false),
                    ClassIndex = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    ShortCutId = table.Column<int>(type: "integer", nullable: false),
                    SkillLevel = table.Column<short>(type: "smallint", nullable: false),
                    SkillSubLevel = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterShortCuts", x => new { x.CharacterId, x.ClassIndex, x.Slot, x.Page });
                });

            migrationBuilder.CreateTable(
                name: "CharacterSkillReuses",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    SkillLevel = table.Column<short>(type: "smallint", nullable: false),
                    ClassIndex = table.Column<byte>(type: "smallint", nullable: false),
                    SkillSubLevel = table.Column<short>(type: "smallint", nullable: false),
                    RemainingTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ReuseDelay = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SysTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RestoreType = table.Column<byte>(type: "smallint", nullable: false),
                    BuffIndex = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSkillReuses", x => new { x.CharacterId, x.ClassIndex, x.SkillId, x.SkillLevel });
                });

            migrationBuilder.CreateTable(
                name: "CharacterSkills",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    ClassIndex = table.Column<byte>(type: "smallint", nullable: false),
                    SkillLevel = table.Column<short>(type: "smallint", nullable: false),
                    SkillSubLevel = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSkills", x => new { x.CharacterId, x.ClassIndex, x.SkillId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterSpirits",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    Stage = table.Column<byte>(type: "smallint", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false),
                    AttackPoints = table.Column<byte>(type: "smallint", nullable: false),
                    DefensePoints = table.Column<byte>(type: "smallint", nullable: false),
                    CriticalRatePoints = table.Column<byte>(type: "smallint", nullable: false),
                    CriticalDamagePoints = table.Column<byte>(type: "smallint", nullable: false),
                    IsInUse = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSpirits", x => new { x.CharacterId, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "CharacterSubClasses",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    SubClass = table.Column<short>(type: "smallint", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false),
                    Sp = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
                    VitalityPoints = table.Column<int>(type: "integer", nullable: false),
                    ClassIndex = table.Column<byte>(type: "smallint", nullable: false),
                    DualClass = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSubClasses", x => new { x.CharacterId, x.SubClass });
                });

            migrationBuilder.CreateTable(
                name: "CharacterSurveillances",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    TargetId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSurveillances", x => new { x.CharacterId, x.TargetId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterTeleportBookmarks",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Z = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<int>(type: "integer", nullable: false),
                    Tag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTeleportBookmarks", x => new { x.CharacterId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "CharacterVariables",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterVariables", x => new { x.CharacterId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "ClanHallBidders",
                columns: table => new
                {
                    ClanHallId = table.Column<int>(type: "integer", nullable: false),
                    ClanId = table.Column<int>(type: "integer", nullable: false),
                    Bid = table.Column<long>(type: "bigint", nullable: false),
                    BidTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanHallBidders", x => new { x.ClanHallId, x.ClanId });
                });

            migrationBuilder.CreateTable(
                name: "ClanHalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    PaidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanHalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClanVariables",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ClanId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanVariables", x => new { x.ClanId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "CommissionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemObjectId = table.Column<int>(type: "integer", nullable: false),
                    PricePerUnit = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationInDays = table.Column<short>(type: "smallint", nullable: false),
                    DiscountInPercentage = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionItems", x => x.Id);
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
                name: "CursedWeapons",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    PlayerReputation = table.Column<int>(type: "integer", nullable: false),
                    PlayerPkKills = table.Column<int>(type: "integer", nullable: false),
                    NbKills = table.Column<int>(type: "integer", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CursedWeapons", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "CustomMails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReceiverId = table.Column<int>(type: "integer", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Items = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomMails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DerbyBets",
                columns: table => new
                {
                    LaneId = table.Column<byte>(type: "smallint", nullable: false),
                    Bet = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerbyBets", x => x.LaneId);
                });

            migrationBuilder.CreateTable(
                name: "DerbyHistory",
                columns: table => new
                {
                    RaceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    First = table.Column<int>(type: "integer", nullable: false),
                    Second = table.Column<int>(type: "integer", nullable: false),
                    OddRate = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerbyHistory", x => x.RaceId);
                });

            migrationBuilder.CreateTable(
                name: "EnchantChallengePointRecharges",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    OptionIndex = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnchantChallengePointRecharges", x => new { x.CharacterId, x.GroupId, x.OptionIndex });
                });

            migrationBuilder.CreateTable(
                name: "EnchantChallengePoints",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnchantChallengePoints", x => new { x.CharacterId, x.GroupId });
                });

            migrationBuilder.CreateTable(
                name: "FortDoorUpgrades",
                columns: table => new
                {
                    DoorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FortId = table.Column<byte>(type: "smallint", nullable: false),
                    Hp = table.Column<int>(type: "integer", nullable: false),
                    PDef = table.Column<int>(type: "integer", nullable: false),
                    MDef = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FortDoorUpgrades", x => x.DoorId);
                });

            migrationBuilder.CreateTable(
                name: "FortFunctions",
                columns: table => new
                {
                    FortId = table.Column<byte>(type: "smallint", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
                    Lease = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FortFunctions", x => new { x.FortId, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "Forts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    SiegeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastOwnedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CastleId = table.Column<int>(type: "integer", nullable: false),
                    SupplyLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FortSiegeClans",
                columns: table => new
                {
                    FortId = table.Column<short>(type: "smallint", nullable: false),
                    ClanId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FortSiegeClans", x => new { x.FortId, x.ClanId });
                });

            migrationBuilder.CreateTable(
                name: "FortSiegeGuards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FortId = table.Column<byte>(type: "smallint", nullable: false),
                    NpcId = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Z = table.Column<int>(type: "integer", nullable: false),
                    Heading = table.Column<int>(type: "integer", nullable: false),
                    RespawnDelay = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsHired = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FortSiegeGuards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FortSpawns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FortId = table.Column<byte>(type: "smallint", nullable: false),
                    NpcId = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Z = table.Column<int>(type: "integer", nullable: false),
                    Heading = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    CastleId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FortSpawns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Post = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    Perm = table.Column<int>(type: "integer", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forums_Forums_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Forums",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GlobalTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TaskType = table.Column<int>(type: "integer", nullable: false),
                    LastRun = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TaskParam1 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TaskParam2 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TaskParam3 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalVariables",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalVariables", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "GrandBosses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Z = table.Column<int>(type: "integer", nullable: false),
                    Heading = table.Column<int>(type: "integer", nullable: false),
                    RespawnTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentHp = table.Column<double>(type: "double precision", nullable: false),
                    CurrentMp = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrandBosses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Heroes",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassId = table.Column<short>(type: "smallint", nullable: false),
                    Count = table.Column<short>(type: "smallint", nullable: false),
                    LegendCount = table.Column<short>(type: "smallint", nullable: false),
                    Played = table.Column<bool>(type: "boolean", nullable: false),
                    Claimed = table.Column<bool>(type: "boolean", nullable: false),
                    Message = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heroes", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "HeroesDiary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Action = table.Column<byte>(type: "smallint", nullable: false),
                    Param = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroesDiary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HuntPasses",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrentStep = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    RewardStep = table.Column<int>(type: "integer", nullable: false),
                    IsPremium = table.Column<bool>(type: "boolean", nullable: false),
                    PremiumRewardStep = table.Column<int>(type: "integer", nullable: false),
                    SayhaPointsAvailable = table.Column<int>(type: "integer", nullable: false),
                    SayhaPointsUsed = table.Column<int>(type: "integer", nullable: false),
                    UnclaimedReward = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntPasses", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "ItemAuctionBids",
                columns: table => new
                {
                    AuctionId = table.Column<int>(type: "integer", nullable: false),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Bid = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAuctionBids", x => new { x.AuctionId, x.CharacterId });
                });

            migrationBuilder.CreateTable(
                name: "ItemAuctions",
                columns: table => new
                {
                    AuctionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstanceId = table.Column<int>(type: "integer", nullable: false),
                    AuctionItemId = table.Column<int>(type: "integer", nullable: false),
                    StartingTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndingTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuctionStateId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAuctions", x => x.AuctionId);
                });

            migrationBuilder.CreateTable(
                name: "ItemElementals",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemElementals", x => new { x.ItemId, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ObjectId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    EnchantLevel = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<int>(type: "integer", nullable: false),
                    LocationData = table.Column<int>(type: "integer", nullable: false),
                    TimeOfUse = table.Column<int>(type: "integer", nullable: false),
                    CustomType1 = table.Column<int>(type: "integer", nullable: false),
                    CustomType2 = table.Column<int>(type: "integer", nullable: false),
                    ManaLeft = table.Column<int>(type: "integer", nullable: true),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ObjectId);
                });

            migrationBuilder.CreateTable(
                name: "ItemsOnGround",
                columns: table => new
                {
                    ObjectId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    EnchantLevel = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Z = table.Column<int>(type: "integer", nullable: false),
                    DropTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Equipable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsOnGround", x => x.ObjectId);
                });

            migrationBuilder.CreateTable(
                name: "ItemSpecialAbilities",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    OptionId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Position = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSpecialAbilities", x => new { x.ItemId, x.OptionId });
                });

            migrationBuilder.CreateTable(
                name: "ItemTransactionHistory",
                columns: table => new
                {
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    EnchantLevel = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTransactionHistory", x => x.CreatedTime);
                });

            migrationBuilder.CreateTable(
                name: "ItemVariables",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemVariables", x => new { x.ItemId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "ItemVariations",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MineralId = table.Column<int>(type: "integer", nullable: false),
                    Option1 = table.Column<int>(type: "integer", nullable: false),
                    Option2 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemVariations", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "MailMessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    ReceiverId = table.Column<int>(type: "integer", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequiredAdena = table.Column<long>(type: "bigint", nullable: false),
                    HasAttachments = table.Column<bool>(type: "boolean", nullable: false),
                    IsUnread = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeletedBySender = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeletedByReceiver = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    SentBySystem = table.Column<byte>(type: "smallint", nullable: false),
                    IsReturned = table.Column<bool>(type: "boolean", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    EnchantLevel = table.Column<short>(type: "smallint", nullable: false),
                    Elementals = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailMessages", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "MerchantLeases",
                columns: table => new
                {
                    MerchantId = table.Column<int>(type: "integer", nullable: false),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Bid = table.Column<int>(type: "integer", nullable: false),
                    CharacterName = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantLeases", x => new { x.MerchantId, x.CharacterId, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "NpcRespawns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Z = table.Column<int>(type: "integer", nullable: false),
                    Heading = table.Column<int>(type: "integer", nullable: false),
                    RespawnTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentHp = table.Column<double>(type: "double precision", nullable: false),
                    CurrentMp = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NpcRespawns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NpcVariables",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NpcId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NpcVariables", x => new { x.NpcId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "OlympiadData",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    CurrentCycle = table.Column<short>(type: "smallint", nullable: false),
                    Period = table.Column<short>(type: "smallint", nullable: false),
                    OlympiadEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidationEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextWeeklyChange = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OlympiadData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OlympiadFights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Character1Id = table.Column<int>(type: "integer", nullable: false),
                    Character2Id = table.Column<int>(type: "integer", nullable: false),
                    Character1Class = table.Column<short>(type: "smallint", nullable: false),
                    Character2Class = table.Column<short>(type: "smallint", nullable: false),
                    Winner = table.Column<byte>(type: "smallint", nullable: false),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Classed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OlympiadFights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OlympiadNobles",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Class = table.Column<short>(type: "smallint", nullable: false),
                    OlympiadPoints = table.Column<int>(type: "integer", nullable: false),
                    CompetitionsDone = table.Column<short>(type: "smallint", nullable: false),
                    CompetitionsWon = table.Column<short>(type: "smallint", nullable: false),
                    CompetitionsLost = table.Column<short>(type: "smallint", nullable: false),
                    CompetitionsDrawn = table.Column<short>(type: "smallint", nullable: false),
                    CompetitionsDoneWeek = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OlympiadNobles", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "OlympiadNoblesEom",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Class = table.Column<short>(type: "smallint", nullable: false),
                    OlympiadPoints = table.Column<int>(type: "integer", nullable: false),
                    CompetitionsDone = table.Column<short>(type: "smallint", nullable: false),
                    CompetitionsWon = table.Column<short>(type: "smallint", nullable: false),
                    CompetitionsLost = table.Column<short>(type: "smallint", nullable: false),
                    CompetitionsDrawn = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OlympiadNoblesEom", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "PartyMatchingHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Leader = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyMatchingHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PetEvolves",
                columns: table => new
                {
                    ItemObjectId = table.Column<int>(type: "integer", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetEvolves", x => new { x.ItemObjectId, x.Index, x.Level });
                });

            migrationBuilder.CreateTable(
                name: "PetSkillReuses",
                columns: table => new
                {
                    PetItemObjectId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    SkillLevel = table.Column<short>(type: "smallint", nullable: false),
                    SkillSubLevel = table.Column<short>(type: "smallint", nullable: false),
                    RemainingTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    BuffIndex = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetSkillReuses", x => new { x.PetItemObjectId, x.SkillId, x.SkillLevel });
                });

            migrationBuilder.CreateTable(
                name: "PetSkills",
                columns: table => new
                {
                    PetItemObjectId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    SkillLevel = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetSkills", x => new { x.PetItemObjectId, x.SkillId, x.SkillLevel });
                });

            migrationBuilder.CreateTable(
                name: "PledgeApplicants",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    ClanId = table.Column<int>(type: "integer", nullable: false),
                    Karma = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PledgeApplicants", x => new { x.CharacterId, x.ClanId });
                });

            migrationBuilder.CreateTable(
                name: "PledgeRecruits",
                columns: table => new
                {
                    ClanId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Karma = table.Column<int>(type: "integer", nullable: false),
                    Information = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DetailedInformation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ApplicationType = table.Column<int>(type: "integer", nullable: false),
                    RecruitType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PledgeRecruits", x => x.ClanId);
                });

            migrationBuilder.CreateTable(
                name: "PledgeWaitingLists",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Karma = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PledgeWaitingLists", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "Punishments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Affect = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    PunishedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Punishments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResidenceFunctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ResidenceId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidenceFunctions", x => new { x.ResidenceId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "SiegeClans",
                columns: table => new
                {
                    CastleId = table.Column<byte>(type: "smallint", nullable: false),
                    ClanId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    CastleOwmer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiegeClans", x => new { x.ClanId, x.CastleId });
                });

            migrationBuilder.CreateTable(
                name: "SummonSkillReuses",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    OwnerClassIndex = table.Column<byte>(type: "smallint", nullable: false),
                    SummonSkillId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    SkillLevel = table.Column<short>(type: "smallint", nullable: false),
                    SkillSubLevel = table.Column<short>(type: "smallint", nullable: false),
                    RemainingTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    BuffIndex = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummonSkillReuses", x => new { x.OwnerId, x.OwnerClassIndex, x.SummonSkillId, x.SkillId, x.SkillLevel });
                });

            migrationBuilder.CreateTable(
                name: "WorldExchangeItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemObjectId = table.Column<int>(type: "integer", nullable: false),
                    ItemStatus = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    OldOwnerId = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldExchangeItems", x => x.Id);
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
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ForumId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OwnerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    Reply = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Forums_ForumId",
                        column: x => x.ForumId,
                        principalTable: "Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TopicId = table.Column<int>(type: "integer", nullable: false),
                    ForumId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Forums_ForumId",
                        column: x => x.ForumId,
                        principalTable: "Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterFriends",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    FriendId = table.Column<int>(type: "integer", nullable: false),
                    Relation = table.Column<int>(type: "integer", nullable: false),
                    Memo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterFriends", x => new { x.CharacterId, x.FriendId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterOfflineTradeItems",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterOfflineTradeItems", x => new { x.CharacterId, x.ItemId });
                });

            migrationBuilder.CreateTable(
                name: "CharacterOfflineTrades",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterOfflineTrades", x => x.CharacterId);
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
                    BaseClass = table.Column<short>(type: "smallint", nullable: false),
                    Class = table.Column<short>(type: "smallint", nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
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
                    PowerGrade = table.Column<int>(type: "integer", nullable: false),
                    IsNobless = table.Column<bool>(type: "boolean", nullable: false),
                    ClanId = table.Column<int>(type: "integer", nullable: true),
                    ClanPrivileges = table.Column<int>(type: "integer", nullable: false),
                    SubPledge = table.Column<int>(type: "integer", nullable: false),
                    SponsorId = table.Column<int>(type: "integer", nullable: true),
                    WantsPeace = table.Column<bool>(type: "boolean", nullable: false),
                    LevelJoinedAcademy = table.Column<byte>(type: "smallint", nullable: false),
                    HasDwarvenCraft = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BirthDay = table.Column<short>(type: "smallint", nullable: false),
                    LastAccess = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OnlineTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    OnlineStatus = table.Column<byte>(type: "smallint", nullable: false),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClanCreateExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClanJoinExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Faction = table.Column<byte>(type: "smallint", nullable: true),
                    Apprentice = table.Column<int>(type: "integer", nullable: false),
                    BookmarkSlot = table.Column<int>(type: "integer", nullable: false),
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
                name: "CharacterSummons",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    SummonId = table.Column<int>(type: "integer", nullable: false),
                    SummonSkillId = table.Column<int>(type: "integer", nullable: false),
                    CurrentHp = table.Column<int>(type: "integer", nullable: false),
                    CurrentMp = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSummons", x => new { x.OwnerId, x.SummonId, x.SummonSkillId });
                    table.ForeignKey(
                        name: "FK_CharacterSummons_Characters_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Castle = table.Column<short>(type: "smallint", nullable: true),
                    BloodAllianceCount = table.Column<short>(type: "smallint", nullable: false),
                    BloodOathCount = table.Column<short>(type: "smallint", nullable: false),
                    AllyId = table.Column<int>(type: "integer", nullable: true),
                    AllyName = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    AllyCrestId = table.Column<int>(type: "integer", nullable: true),
                    LeaderId = table.Column<int>(type: "integer", nullable: false),
                    CrestId = table.Column<int>(type: "integer", nullable: true),
                    LargeCrestId = table.Column<int>(type: "integer", nullable: true),
                    AuctionBidAt = table.Column<int>(type: "integer", nullable: false),
                    AllyPenaltyExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AllyPenaltyExpireType = table.Column<byte>(type: "smallint", nullable: false),
                    CharPenaltyExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DissolvingExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NewLeaderId = table.Column<int>(type: "integer", nullable: true),
                    Exp = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clans", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    ItemObjectId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
                    CurrentHp = table.Column<int>(type: "integer", nullable: false),
                    CurrentMp = table.Column<int>(type: "integer", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false),
                    Sp = table.Column<long>(type: "bigint", nullable: false),
                    Fed = table.Column<int>(type: "integer", nullable: false),
                    Restore = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.OwnerId);
                    table.ForeignKey(
                        name: "FK_Pets_Characters_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanNotices",
                columns: table => new
                {
                    ClanId = table.Column<int>(type: "integer", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Notice = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanNotices", x => x.ClanId);
                    table.ForeignKey(
                        name: "FK_ClanNotices_Clans_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanPrivileges",
                columns: table => new
                {
                    ClanId = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Party = table.Column<int>(type: "integer", nullable: false),
                    Privileges = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanPrivileges", x => new { x.ClanId, x.Rank, x.Party });
                    table.ForeignKey(
                        name: "FK_ClanPrivileges_Clans_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanSkills",
                columns: table => new
                {
                    ClanId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    SubPledgeId = table.Column<int>(type: "integer", nullable: false),
                    SkillLevel = table.Column<short>(type: "smallint", nullable: false),
                    SkillName = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanSkills", x => new { x.ClanId, x.SkillId, x.SubPledgeId });
                    table.ForeignKey(
                        name: "FK_ClanSkills_Clans_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanSubPledges",
                columns: table => new
                {
                    ClanId = table.Column<int>(type: "integer", nullable: false),
                    SubPledgeId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    LeaderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanSubPledges", x => new { x.ClanId, x.SubPledgeId });
                    table.ForeignKey(
                        name: "FK_ClanSubPledges_Characters_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanSubPledges_Clans_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanWars",
                columns: table => new
                {
                    Clan1Id = table.Column<int>(type: "integer", nullable: false),
                    Clan2Id = table.Column<int>(type: "integer", nullable: false),
                    Clan1Kills = table.Column<int>(type: "integer", nullable: false),
                    Clan2Kills = table.Column<int>(type: "integer", nullable: false),
                    WinnerClanId = table.Column<int>(type: "integer", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    State = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanWars", x => new { x.Clan1Id, x.Clan2Id });
                    table.ForeignKey(
                        name: "FK_ClanWars_Clans_Clan1Id",
                        column: x => x.Clan1Id,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanWars_Clans_Clan2Id",
                        column: x => x.Clan2Id,
                        principalTable: "Clans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanWars_Clans_WinnerClanId",
                        column: x => x.WinnerClanId,
                        principalTable: "Clans",
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
                name: "IX_Castles_Id",
                table: "Castles",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterFriends_FriendId",
                table: "CharacterFriends",
                column: "FriendId");

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
                name: "IX_CharacterShortCuts_ShortCutId",
                table: "CharacterShortCuts",
                column: "ShortCutId");

            migrationBuilder.CreateIndex(
                name: "IX_ClanHalls_OwnerId",
                table: "ClanHalls",
                column: "OwnerId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ClanSubPledges_LeaderId",
                table: "ClanSubPledges",
                column: "LeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClanWars_Clan2Id",
                table: "ClanWars",
                column: "Clan2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClanWars_WinnerClanId",
                table: "ClanWars",
                column: "WinnerClanId");

            migrationBuilder.CreateIndex(
                name: "IX_CursedWeapons_CharacterId",
                table: "CursedWeapons",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forts_OwnerId",
                table: "Forts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FortSiegeGuards_FortId",
                table: "FortSiegeGuards",
                column: "FortId");

            migrationBuilder.CreateIndex(
                name: "IX_FortSpawns_FortId",
                table: "FortSpawns",
                column: "FortId");

            migrationBuilder.CreateIndex(
                name: "IX_Forums_ParentId",
                table: "Forums",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroesDiary_CharacterId",
                table: "HeroesDiary",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemId",
                table: "Items",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_OwnerId_ItemId_Location",
                table: "Items",
                columns: new[] { "OwnerId", "ItemId", "Location" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_OwnerId_Location",
                table: "Items",
                columns: new[] { "OwnerId", "Location" });

            migrationBuilder.CreateIndex(
                name: "IX_OlympiadFights_Character1Id",
                table: "OlympiadFights",
                column: "Character1Id");

            migrationBuilder.CreateIndex(
                name: "IX_OlympiadFights_Character2Id",
                table: "OlympiadFights",
                column: "Character2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_ItemObjectId",
                table: "Pets",
                column: "ItemObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ForumId",
                table: "Posts",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TopicId",
                table: "Posts",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_ForumId",
                table: "Topics",
                column: "ForumId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterFriends_Characters_CharacterId",
                table: "CharacterFriends",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterFriends_Characters_FriendId",
                table: "CharacterFriends",
                column: "FriendId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterOfflineTradeItems_Characters_CharacterId",
                table: "CharacterOfflineTradeItems",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterOfflineTrades_Characters_CharacterId",
                table: "CharacterOfflineTrades",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Clans_Crests_CrestId",
                table: "Clans");

            migrationBuilder.DropForeignKey(
                name: "FK_Clans_Crests_LargeCrestId",
                table: "Clans");

            migrationBuilder.DropForeignKey(
                name: "FK_Clans_Characters_LeaderId",
                table: "Clans");

            migrationBuilder.DropTable(
                name: "AccountCollectionFavorites");

            migrationBuilder.DropTable(
                name: "AccountCollections");

            migrationBuilder.DropTable(
                name: "AccountPremiums");

            migrationBuilder.DropTable(
                name: "AccountVariables");

            migrationBuilder.DropTable(
                name: "AchievementBoxes");

            migrationBuilder.DropTable(
                name: "AirShips");

            migrationBuilder.DropTable(
                name: "Allys");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "BotReports");

            migrationBuilder.DropTable(
                name: "BufferSchemes");

            migrationBuilder.DropTable(
                name: "BuyLists");

            migrationBuilder.DropTable(
                name: "CastleDoorUpgrades");

            migrationBuilder.DropTable(
                name: "CastleFunctions");

            migrationBuilder.DropTable(
                name: "CastleManorProcure");

            migrationBuilder.DropTable(
                name: "CastleManorProduction");

            migrationBuilder.DropTable(
                name: "Castles");

            migrationBuilder.DropTable(
                name: "CastleSiegeClans");

            migrationBuilder.DropTable(
                name: "CastleSiegeGuards");

            migrationBuilder.DropTable(
                name: "CastleTrapUpgrades");

            migrationBuilder.DropTable(
                name: "CharacterContacts");

            migrationBuilder.DropTable(
                name: "CharacterCouples");

            migrationBuilder.DropTable(
                name: "CharacterDailyRewards");

            migrationBuilder.DropTable(
                name: "CharacterFriends");

            migrationBuilder.DropTable(
                name: "CharacterHennaPotens");

            migrationBuilder.DropTable(
                name: "CharacterHennas");

            migrationBuilder.DropTable(
                name: "CharacterInstances");

            migrationBuilder.DropTable(
                name: "CharacterItemReuses");

            migrationBuilder.DropTable(
                name: "CharacterMacros");

            migrationBuilder.DropTable(
                name: "CharacterMentees");

            migrationBuilder.DropTable(
                name: "CharacterOfflineTradeItems");

            migrationBuilder.DropTable(
                name: "CharacterOfflineTrades");

            migrationBuilder.DropTable(
                name: "CharacterPremiumItems");

            migrationBuilder.DropTable(
                name: "CharacterPurges");

            migrationBuilder.DropTable(
                name: "CharacterQuests");

            migrationBuilder.DropTable(
                name: "CharacterRandomCrafts");

            migrationBuilder.DropTable(
                name: "CharacterRankingHistory");

            migrationBuilder.DropTable(
                name: "CharacterRecipeBooks");

            migrationBuilder.DropTable(
                name: "CharacterRecipeShopLists");

            migrationBuilder.DropTable(
                name: "CharacterRecoBonuses");

            migrationBuilder.DropTable(
                name: "CharacterRevenges");

            migrationBuilder.DropTable(
                name: "CharacterShortCuts");

            migrationBuilder.DropTable(
                name: "CharacterSkillReuses");

            migrationBuilder.DropTable(
                name: "CharacterSkills");

            migrationBuilder.DropTable(
                name: "CharacterSpirits");

            migrationBuilder.DropTable(
                name: "CharacterSubClasses");

            migrationBuilder.DropTable(
                name: "CharacterSummons");

            migrationBuilder.DropTable(
                name: "CharacterSurveillances");

            migrationBuilder.DropTable(
                name: "CharacterTeleportBookmarks");

            migrationBuilder.DropTable(
                name: "CharacterVariables");

            migrationBuilder.DropTable(
                name: "ClanHallBidders");

            migrationBuilder.DropTable(
                name: "ClanHalls");

            migrationBuilder.DropTable(
                name: "ClanNotices");

            migrationBuilder.DropTable(
                name: "ClanPrivileges");

            migrationBuilder.DropTable(
                name: "ClanSkills");

            migrationBuilder.DropTable(
                name: "ClanSubPledges");

            migrationBuilder.DropTable(
                name: "ClanVariables");

            migrationBuilder.DropTable(
                name: "ClanWars");

            migrationBuilder.DropTable(
                name: "CommissionItems");

            migrationBuilder.DropTable(
                name: "CursedWeapons");

            migrationBuilder.DropTable(
                name: "CustomMails");

            migrationBuilder.DropTable(
                name: "DerbyBets");

            migrationBuilder.DropTable(
                name: "DerbyHistory");

            migrationBuilder.DropTable(
                name: "EnchantChallengePointRecharges");

            migrationBuilder.DropTable(
                name: "EnchantChallengePoints");

            migrationBuilder.DropTable(
                name: "FortDoorUpgrades");

            migrationBuilder.DropTable(
                name: "FortFunctions");

            migrationBuilder.DropTable(
                name: "Forts");

            migrationBuilder.DropTable(
                name: "FortSiegeClans");

            migrationBuilder.DropTable(
                name: "FortSiegeGuards");

            migrationBuilder.DropTable(
                name: "FortSpawns");

            migrationBuilder.DropTable(
                name: "GlobalTasks");

            migrationBuilder.DropTable(
                name: "GlobalVariables");

            migrationBuilder.DropTable(
                name: "GrandBosses");

            migrationBuilder.DropTable(
                name: "Heroes");

            migrationBuilder.DropTable(
                name: "HeroesDiary");

            migrationBuilder.DropTable(
                name: "HuntPasses");

            migrationBuilder.DropTable(
                name: "ItemAuctionBids");

            migrationBuilder.DropTable(
                name: "ItemAuctions");

            migrationBuilder.DropTable(
                name: "ItemElementals");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "ItemsOnGround");

            migrationBuilder.DropTable(
                name: "ItemSpecialAbilities");

            migrationBuilder.DropTable(
                name: "ItemTransactionHistory");

            migrationBuilder.DropTable(
                name: "ItemVariables");

            migrationBuilder.DropTable(
                name: "ItemVariations");

            migrationBuilder.DropTable(
                name: "MailMessages");

            migrationBuilder.DropTable(
                name: "MerchantLeases");

            migrationBuilder.DropTable(
                name: "NpcRespawns");

            migrationBuilder.DropTable(
                name: "NpcVariables");

            migrationBuilder.DropTable(
                name: "OlympiadData");

            migrationBuilder.DropTable(
                name: "OlympiadFights");

            migrationBuilder.DropTable(
                name: "OlympiadNobles");

            migrationBuilder.DropTable(
                name: "OlympiadNoblesEom");

            migrationBuilder.DropTable(
                name: "PartyMatchingHistory");

            migrationBuilder.DropTable(
                name: "PetEvolves");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "PetSkillReuses");

            migrationBuilder.DropTable(
                name: "PetSkills");

            migrationBuilder.DropTable(
                name: "PledgeApplicants");

            migrationBuilder.DropTable(
                name: "PledgeRecruits");

            migrationBuilder.DropTable(
                name: "PledgeWaitingLists");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Punishments");

            migrationBuilder.DropTable(
                name: "ResidenceFunctions");

            migrationBuilder.DropTable(
                name: "SiegeClans");

            migrationBuilder.DropTable(
                name: "SummonSkillReuses");

            migrationBuilder.DropTable(
                name: "WorldExchangeItems");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Forums");

            migrationBuilder.DropTable(
                name: "Crests");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "AccountRefs");

            migrationBuilder.DropTable(
                name: "Clans");
        }
    }
}
