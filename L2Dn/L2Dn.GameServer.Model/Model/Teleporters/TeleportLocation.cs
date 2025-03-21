using System.Collections.Immutable;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;
using L2Dn.Model.Xml;

namespace L2Dn.GameServer.Model.Teleporters;

public class TeleportLocation
{
    private readonly Location3D _location;
    private readonly int _id;
    private readonly string _name;
    private readonly NpcStringId? _npcStringId;
    private readonly int _questZoneId;
    private readonly int _feeId;
    private readonly long _feeCount;
    private readonly ImmutableArray<int> _castleId = [];
    
    public TeleportLocation(int id, XmlTeleportLocation location)
    {
        _id = id;
        _location = new Location3D(location.X, location.Y, location.Z);
        _name = location.NameSpecified ? location.Name : string.Empty;
        _npcStringId = location.NpcStringIdSpecified ? (NpcStringId)location.NpcStringId : null;
        _questZoneId = location.QuestZoneIdSpecified ? location.QuestZoneId : 0;
        _feeId =  location.FeeItemIdSpecified ? location.FeeItemId : Inventory.AdenaId;
        _feeCount = location.FeeCountSpecified ? location.FeeCount : 0;

        if (location.CastleIdSpecified && !string.IsNullOrEmpty(location.CastleId))
            _castleId = location.CastleId.Split(';').Select(int.Parse).ToImmutableArray();
    }

    public Location3D Location => _location;

    public int getId()
    {
        return _id;
    }

    public string getName()
    {
        return _name;
    }

    public NpcStringId? getNpcStringId()
    {
        return _npcStringId;
    }

    public int getQuestZoneId()
    {
        return _questZoneId;
    }

    public int getFeeId()
    {
        return _feeId;
    }

    public long getFeeCount()
    {
        return _feeCount;
    }

    public ImmutableArray<int> getCastleId()
    {
        return _castleId;
    }
}