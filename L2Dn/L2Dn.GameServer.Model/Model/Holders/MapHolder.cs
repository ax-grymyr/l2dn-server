namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class MapHolder
{
	private readonly int _x;
	private readonly int _y;
	
	public MapHolder(int x, int y)
	{
		_x = x;
		_y = y;
	}
	
	public int getX()
	{
		return _x;
	}
	
	public long getY()
	{
		return _y;
	}
	
	public override string ToString()
	{
		return "[" + GetType().Name + "] X: " + _x + ", Y: " + _y;
	}
}