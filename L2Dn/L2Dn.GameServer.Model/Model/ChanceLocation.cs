using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

public class ChanceLocation
{
    private readonly LocationHeading _location;
    private readonly double _chance;

    public ChanceLocation(LocationHeading location, double chance)
    {
        _location = location;
        _chance = chance;
    }

    public LocationHeading Location => _location;

    public double getChance()
    {
        return _chance;
    }
}
