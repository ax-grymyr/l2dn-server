namespace L2Dn.GameServer.Model;

public class WalkRoute
{
    private readonly string _name;
    private readonly List<NpcWalkerNode> _nodeList; // List of nodes
    private readonly bool _repeatWalk; // Does repeat walk, after arriving into last point in list, or not
    private readonly bool _stopAfterCycle; // Make only one cycle or endlessly

    // Repeat style: 0 - go back, 1 - go to first point (circle style), 2 - teleport to first point (conveyor style), 3 - random walking between points
    // TODO: 1) make enum, 2) possibly _stopAfterCycle can be encoded as None into the enum
    private readonly byte _repeatType;

    public WalkRoute(string name, List<NpcWalkerNode> route, bool repeat, byte repeatType)
    {
        _name = name;
        _nodeList = route;
        _repeatType = repeatType;
        _stopAfterCycle = !repeat;
        _repeatWalk = _repeatType <= 2 && repeat;
    }

    public string getName()
    {
        return _name;
    }

    public List<NpcWalkerNode> getNodeList()
    {
        return _nodeList;
    }

    public NpcWalkerNode getLastNode()
    {
        return _nodeList[_nodeList.Count - 1];
    }

    public bool repeatWalk()
    {
        return _repeatWalk;
    }

    public bool doOnce()
    {
        return _stopAfterCycle;
    }

    public byte getRepeatType()
    {
        return _repeatType;
    }

    public int getNodesCount()
    {
        return _nodeList.Count;
    }
}