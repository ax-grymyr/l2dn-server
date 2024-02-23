namespace L2Dn.GameServer.Db;

public struct CharacterRandomCraftItem
{
    public int Id { get; set; }
    public long Count { get; set; }
    public bool Locked { get; set; }
    public int LockLeft { get; set; }
}