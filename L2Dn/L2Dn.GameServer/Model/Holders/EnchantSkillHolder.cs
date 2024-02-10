using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;

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
	
	public EnchantSkillHolder(StatSet set)
	{
		_level = set.getInt("level");
		_enchantFailLevel = set.getInt("enchantFailLevel");
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
		if (_chance.TryGetValue(type, out int val))
		{
			return val;
		}
		
		return 100;
	}
	
	public void addRequiredItem(SkillEnchantType type, ItemHolder item)
	{
		_requiredItems.computeIfAbsent(type, k => new()).add(item);
	}
	
	public Set<ItemHolder> getRequiredItems(SkillEnchantType type)
	{
		return _requiredItems.getOrDefault(type, Collections.emptySet());
	}
}
