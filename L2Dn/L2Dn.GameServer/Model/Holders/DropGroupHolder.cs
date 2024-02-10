namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class DropGroupHolder
{
	private readonly List<DropHolder> _dropList = new();
	private readonly double _chance;

	public DropGroupHolder(double chance)
	{
		_chance = chance;
	}

	public List<DropHolder> getDropList()
	{
		return _dropList;
	}

	public void addDrop(DropHolder holder)
	{
		_dropList.Add(holder);
	}

	public double getChance()
	{
		return _chance;
	}
}