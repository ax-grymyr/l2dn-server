namespace L2Dn.GameServer.Enums;

[Flags]
public enum PartySmallWindowUpdateType: short
{
    None = 0,

    CURRENT_CP = 1,
    MAX_CP = 2,
    CURRENT_HP = 4,
    MAX_HP = 8,
    CURRENT_MP = 16,
    MAX_MP = 32,
    LEVEL = 64,
    CLASS_ID = 128,
    PARTY_SUBSTITUTE = 256,
    VITALITY_POINTS = 512,

    All = CURRENT_CP | MAX_CP | CURRENT_HP | MAX_HP | CURRENT_MP | MAX_MP | LEVEL | CLASS_ID | PARTY_SUBSTITUTE |
          VITALITY_POINTS
}