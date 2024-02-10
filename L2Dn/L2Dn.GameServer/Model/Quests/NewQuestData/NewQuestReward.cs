using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Quests.NewQuestData;

/**
 * @author Magik
 */
public class NewQuestReward
{
	private readonly long _exp;
	private readonly long _sp;
	private readonly int _level;
	private readonly List<ItemHolder> _items;

	public NewQuestReward(long exp, long sp, int level, List<ItemHolder> items)
	{
		_exp = exp;
		_sp = sp;
		_level = level;
		_items = items;
	}

	public long getExp()
	{
		return _exp;
	}

	public long getSp()
	{
		return _sp;
	}

	public int getLevel()
	{
		return _level;
	}

	public List<ItemHolder> getItems()
	{
		return _items;
	}
}