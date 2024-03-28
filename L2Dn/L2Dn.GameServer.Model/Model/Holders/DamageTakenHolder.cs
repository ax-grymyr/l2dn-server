using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class DamageTakenHolder
{
	private readonly Creature _creature;
	private readonly int _skillId;
	private readonly double _damage;

	public DamageTakenHolder(Creature creature, int skillId, double damage)
	{
		_creature = creature;
		_skillId = skillId;
		_damage = damage;
	}

	public Creature getCreature()
	{
		return _creature;
	}

	public int getSkillId()
	{
		return _skillId;
	}

	public double getDamage()
	{
		return _damage;
	}
}