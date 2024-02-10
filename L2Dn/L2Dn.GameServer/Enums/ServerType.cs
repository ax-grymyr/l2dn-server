namespace L2Dn.GameServer.Enums;

[Flags]
public enum ServerType
{
    None = 0,
    Normal = 0b1,
    Relax = 0b10,
    Test = 0b100,
    Broad = 0b1000,
    Restricted = 0b1_0000,
    Event = 0b10_0000,
    Free = 0b100_0000,
    World = 0b1_0000_0000,
    New = 0b10_0000_0000,
    Essence = 0b100_0000_0000
}