using System.Xml.Linq;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Sdw
 */
public class EnchantSkillHolder
{
	private readonly int _level;
	private readonly int _enchantFailLevel;
	private readonly Map<SkillEnchantType, long> _sp = new();
	private readonly Map<SkillEnchantType, int> _chance = new();
	private readonly Map<SkillEnchantType, Set<ItemHolder>> _requiredItems = new();

	public EnchantSkillHolder(int level, int enchantFailLevel)
	{
		_level = level;
		_enchantFailLevel = enchantFailLevel;
	}

	public int getLevel()
	{
		return _level;
	}

	public int getEnchantFailLevel()
	{
		return _enchantFailLevel;
	}

	public void addSp(SkillEnchantType type, long sp)
	{
		_sp.put(type, sp);
	}

	public long getSp(SkillEnchantType type)
	{
		long val = _sp.get(type);
		return val;
	}

	public void addChance(SkillEnchantType type, int chance)
	{
		_chance.put(type, chance);
	}

	public int getChance(SkillEnchantType type)
	{
		return _chance.GetValueOrDefault(type, 100);
	}

	public void addRequiredItem(SkillEnchantType type, ItemHolder item)
	{
		_requiredItems.GetOrAdd(type, _ => []).add(item);
	}

	public Set<ItemHolder> getRequiredItems(SkillEnchantType type)
	{
		return _requiredItems.GetValueOrDefault(type, []);
	}
}