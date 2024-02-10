namespace L2Dn.GameServer.Enums;

public enum StatType
{
    HP,
    MP,
    XP,
    SP,
    GIM // grab item modifier:
    // GIM: the default function uses only the skilllevel to determine
    // how many item is grabbed in each step
    // with this stat changer you can multiple this
}