namespace L2Dn.GameServer.Model;

public class ChanceLocation
{
    private readonly Location _location;
    private readonly double _chance;

    public ChanceLocation(Location location, double chance)
    {
        _location = location;
        _chance = chance;
    }

    public Location Location => _location;

    public double getChance()
    {
        return _chance;
    }
}
