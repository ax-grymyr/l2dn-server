namespace L2Dn;

[Flags]
public enum GameServerAttributes
{
    None = 0,
    Normal = 0b1,
    Relax = 0b10,
    PublicTest = 0b100,
    NoLabel = 0b1000,
    CharacterCreationRestricted = 0b1_0000,
    Event = 0b10_0000,
    Free = 0b100_0000,
    World = 0b1_0000_0000,
    New = 0b10_0000_0000,
    EssenceOrClassic = 0b100_0000_0000,
}