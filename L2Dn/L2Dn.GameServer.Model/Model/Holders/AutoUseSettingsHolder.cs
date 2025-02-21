using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class AutoUseSettingsHolder
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AutoUseSettingsHolder));
	private readonly Set<int> _autoSupplyItems = [];
	private readonly Set<int> _autoActions = [];
	private readonly Set<int> _autoBuffs = [];
	private readonly List<int> _autoSkills = [];
	private int _autoPotionItem;
	private int _autoPetPotionItem;
	private int _skillIndex;

	public ICollection<int> getAutoSupplyItems()
	{
		return _autoSupplyItems;
	}

	public ICollection<int> getAutoActions()
	{
		return _autoActions;
	}

	public ICollection<int> getAutoBuffs()
	{
		return _autoBuffs;
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
		return _autoSkills.Contains(skillId) || _autoBuffs.Contains(skillId);
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
            _logger.Error(e);
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
		return _autoSupplyItems.isEmpty() && _autoPotionItem == 0 && _autoPetPotionItem == 0 &&
		       _autoSkills.Count == 0 && _autoBuffs.isEmpty() && _autoActions.isEmpty();
	}
}