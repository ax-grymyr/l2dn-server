namespace L2Dn.GameServer.Db;

public class DbGrandBoss
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int Heading { get; set; }
    public DateTime RespawnTime { get; set; }
    public double CurrentHp { get; set; }
    public double CurrentMp { get; set; }
    public byte Status { get; set; }
}