using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class AutoUseSettingsHolder
{
	private readonly Set<int> _autoSupplyItems = new();
	private readonly Set<int> _autoActions = new();
	private readonly Set<int> _autoBuffs = new();
	private readonly List<int> _autoSkills = new();
	private int _autoPotionItem;
	private int _autoPetPotionItem;
	private int _skillIndex;

	public AutoUseSettingsHolder()
	{
	}

	public ICollection<int> getAutoSupplyItems()
	{
		return _autoSupplyItems.Keys;
	}

	public ICollection<int> getAutoActions()
	{
		return _autoActions.Keys;
	}

	public ICollection<int> getAutoBuffs()
	{
		return _autoBuffs.Keys;
	}

	public List<int> getAutoSkills()
	{
		return _autoSkills;
	}

	public int getAutoPotionItem()
	{
		return _autoPotionItem;
	}

	public void setAutoPotionItem(int itemId)
	{
		_autoPotionItem = itemId;
	}

	public int getAutoPetPotionItem()
	{
		return _autoPetPotionItem;
	}

	public void setAutoPetPotionItem(int itemId)
	{
		_autoPetPotionItem = itemId;
	}

	public bool isAutoSkill(int skillId)
	{
		return _autoSkills.Contains(skillId) || _autoBuffs.contains(skillId);
	}

	public int getNextSkillId()
	{
		if (_skillIndex >= _autoSkills.Count)
		{
			_skillIndex = 0;
		}

		int skillId = int.MinValue;
		try
		{
			skillId = _autoSkills[_skillIndex];
		}
		catch (Exception e)
		{
			resetSkillOrder();
		}

		return skillId;
	}

	public void incrementSkillOrder()
	{
		_skillIndex++;
	}

	public void resetSkillOrder()
	{
		_skillIndex = 0;
	}

	public bool isEmpty()
	{
		return _autoSupplyItems.isEmpty() && (_autoPotionItem == 0) && (_autoPetPotionItem == 0) &&
		       _autoSkills.Count == 0 && _autoBuffs.isEmpty() && _autoActions.isEmpty();
	}
}