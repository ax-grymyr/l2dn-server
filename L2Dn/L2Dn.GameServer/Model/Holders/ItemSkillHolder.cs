using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class ItemSkillHolder: SkillHolder
{
	private readonly ItemSkillType _type;
	private readonly int _chance;
	private readonly int _value;
	
	public ItemSkillHolder(int skillId, int skillLevel, ItemSkillType type, int chance, int value): base(skillId, skillLevel)
	{
		_type = type;
		_chance = chance;
		_value = value;
	}
	
	public ItemSkillType getType()
	{
		return _type;
	}
	
	public int getChance()
	{
		return _chance;
	}
	
	public int getValue()
	{
		return _value;
	}
}