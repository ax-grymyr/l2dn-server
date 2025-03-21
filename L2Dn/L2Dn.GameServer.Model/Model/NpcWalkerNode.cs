using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

/**
 * @author Rayan RPG, JIV
 * @since 927
 */
public class NpcWalkerNode
{
    private readonly Location3D _location;
    private readonly string _chatString;
    private readonly NpcStringId _npcString;
    private readonly int _delay;
    private readonly bool _runToLocation;

    public NpcWalkerNode(Location3D moveLocation, int delay, bool runToLocation, NpcStringId npcString, string chatText)
    {
        _location = moveLocation;
        _delay = delay;
        _runToLocation = runToLocation;
        _npcString = npcString;
        _chatString = chatText;
    }

    public Location3D Location => _location;

    public int getDelay()
    {
        return _delay;
    }

    public bool runToLocation()
    {
        return _runToLocation;
    }

    public NpcStringId getNpcString()
    {
        return _npcString;
    }

    public string getChatText()
    {
        if (_npcString != 0)
        {
            throw new InvalidOperationException("npcString is defined for walker route!");
        }

        return _chatString;
    }
}