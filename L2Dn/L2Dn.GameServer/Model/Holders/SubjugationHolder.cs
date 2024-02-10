using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Berezkin Nikolay
 */
public class SubjugationHolder
{
	private readonly int _category;
	private readonly List<int[]> _hottimes;
	private readonly Map<int, int> _npcs;

	public SubjugationHolder(int category, List<int[]> hottimes, Map<int, int> npcs)
	{
		_category = category;
		_hottimes = hottimes;
		_npcs = npcs;
	}

	public int getCategory()
	{
		return _category;
	}

	public List<int[]> getHottimes()
	{
		return _hottimes;
	}

	public Map<int, int> getNpcs()
	{
		return _npcs;
	}
}