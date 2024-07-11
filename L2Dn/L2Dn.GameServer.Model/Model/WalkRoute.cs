namespace L2Dn.GameServer.Model;

public class WalkRoute
{
    private readonly string _name;
    private readonly List<NpcWalkerNode> _nodeList; // List of nodes
    private readonly bool _repeatWalk; // Does repeat walk, after arriving into last point in list, or not
    private bool _stopAfterCycle; // Make only one cycle or endlessly
    private byte _repeatType; // Repeat style: 0 - go back, 1 - go to first point (circle style), 2 - teleport to first point (conveyor style), 3 - random walking between points
	
    public WalkRoute(string name, List<NpcWalkerNode> route, bool repeat, byte repeatType)
    {
        _name = name;
        _nodeList = route;
        _repeatType = repeatType;
        _repeatWalk = (_repeatType >= 0) && (_repeatType <= 2) && repeat;
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
