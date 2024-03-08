using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Utilities;

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

	public NewQuestReward(XElement? element)
	{
		long rewardExp = -1;
		long rewardSp = -1;
		int rewardLevel = -1;
		List<ItemHolder> rewardItems = new();
		
		element?.Elements("param").ForEach(el =>
		{
			string name = el.GetAttributeValueAsString("name");
			switch (name)
			{
				case "rewardExp":
				{
					rewardExp = (long)el;
					break;
				}
				case "rewardSp":
				{
					rewardSp = (long)el;
					break;
				}
				case "rewardLevel":
				{
					rewardLevel = (int)el;
					break;
				}
			}
		});
		
		element?.Elements("items").Elements("item").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("id");
			int itemCount = el.GetAttributeValueAsInt32("count");
			ItemHolder rewardItem = new ItemHolder(itemId, itemCount);
			rewardItems.Add(rewardItem);
		});

		_exp = rewardExp;
		_sp = rewardSp;
		_level = rewardLevel;
		_items = rewardItems;
	}
		
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