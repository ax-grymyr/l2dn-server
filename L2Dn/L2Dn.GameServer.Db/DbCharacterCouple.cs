namespace L2Dn.GameServer.Db;

public class DbCharacterCouple
{
    // TODO: merge tables friends, contacts and couples
    public int Id { get; set; }
    public int Character1Id { get; set; }
    public int Character2Id { get; set; }
    public bool Married { get; set; }
    public DateTime AffianceDate { get; set; }
    public DateTime? WeddingDate { get; set; }
}