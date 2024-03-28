namespace L2Dn.GameServer.Model;

public class ChanceLocation: Location
{
    private readonly double _chance;

    public ChanceLocation(int x, int y, int z, int heading, double chance): base(x, y, z, heading)
    {
        _chance = chance;
    }

    public double getChance()
    {
        return _chance;
    }
}
