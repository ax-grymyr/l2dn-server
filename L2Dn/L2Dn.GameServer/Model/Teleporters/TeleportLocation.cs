using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Teleporters;

public class TeleportLocation: Location
{
    private readonly int _id;
    private readonly String _name;
    private readonly NpcStringId _npcStringId;
    private readonly int _questZoneId;
    private readonly int _feeId;
    private readonly long _feeCount;
    private readonly List<int> _castleId;

    public TeleportLocation(int id, StatSet set): base(set)
    {
        _id = id;
        _name = set.getString("name", null);
        _npcStringId = NpcStringId.getNpcStringIdOrDefault(set.getInt("npcStringId", -1), null);
        _questZoneId = set.getInt("questZoneId", 0);
        _feeId = set.getInt("feeId", Inventory.ADENA_ID);
        _feeCount = set.getLong("feeCount", 0);

        String castleIds = set.getString("castleId", "");
        if (castleIds.isEmpty())
        {
            _castleId = new();
        }
        else if (!castleIds.Contains(";"))
        {
            _castleId = [int.Parse(castleIds)];
        }
        else
        {
            _castleId = new();
            foreach (String castleId in castleIds.Split(";"))
            {
                _castleId.add(int.Parse(castleId));
            }
        }
    }

    public int getId()
    {
        return _id;
    }

    public String getName()
    {
        return _name;
    }

    public NpcStringId getNpcStringId()
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

    public List<int> getCastleId()
    {
        return _castleId;
    }
}