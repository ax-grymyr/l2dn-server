namespace L2Dn.GameServer.Enums;

[Flags]
public enum GroupType
{
    None = 0,
    
    Player = 1,
    PARTY = 2,
    COMMAND_CHANNEL = 4
}