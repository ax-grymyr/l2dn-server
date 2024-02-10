namespace L2Dn.AuthServer.Db;

[Flags]
public enum GameServerAttributes
{
    None = 0,
    Normal = 1,
    Relax = 2,
    PublicTest = 4,
    NoLabel = 8,
    CharacterCreationRestricted = 16,
    Event = 32,
    Free = 64,
}