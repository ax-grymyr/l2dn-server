namespace L2Dn.GameServer.Network.Enums;

public enum SystemMessageParamType: byte
{
    TYPE_TEXT = 0,
    TYPE_INT_NUMBER = 1,
    TYPE_NPC_NAME = 2,
    TYPE_ITEM_NAME = 3,
    TYPE_SKILL_NAME = 4,
    TYPE_CASTLE_NAME = 5,
    TYPE_LONG_NUMBER = 6,
    TYPE_ZONE_NAME = 7,

    // id 8 - ddd
    TYPE_ELEMENT_NAME = 9,
    TYPE_INSTANCE_NAME = 10,
    TYPE_DOOR_NAME = 11,
    TYPE_PLAYER_NAME = 12,
    TYPE_SYSTEM_STRING = 13,

    // id 14 dSSSSS
    TYPE_CLASS_ID = 15,
    TYPE_POPUP_ID = 16,

    // id 17 shared with 1-3,17,22
    // id 18 Q (read same as 6)
    // id 19 c
    // id 20 c
    // id 21 h
    // id 22 d (shared with 1-3,17,22
    TYPE_BYTE = 20,
    TYPE_FACTION_NAME = 24, // c(short), faction id.
    TYPE_ELEMENTAL_SPIRIT = 26,
}