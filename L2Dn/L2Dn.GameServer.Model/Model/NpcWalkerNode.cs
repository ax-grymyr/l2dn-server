using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model;

/**
 * @author Rayan RPG, JIV
 * @since 927
 */
public class NpcWalkerNode: Location
{
    private readonly String _chatString;
    private readonly NpcStringId _npcString;
    private readonly int _delay;
    private readonly bool _runToLocation;

    public NpcWalkerNode(int moveX, int moveY, int moveZ, int delay, bool runToLocation, NpcStringId npcString,
        String chatText): base(moveX, moveY, moveZ)
    {
        _delay = delay;
        _runToLocation = runToLocation;
        _npcString = npcString;
        _chatString = (chatText == null) ? "" : chatText;
    }

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

    public String getChatText()
    {
        if (_npcString != null)
        {
            throw new InvalidOperationException("npcString is defined for walker route!");
        }

        return _chatString;
    }
}
