using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerSkillLearn: EventBase
{
	private readonly Npc? _trainer;
	private readonly Player _player;
	private readonly Skill _skill;
	private readonly AcquireSkillType _type;

	public OnPlayerSkillLearn(Npc? trainer, Player player, Skill skill, AcquireSkillType type)
	{
		_trainer = trainer;
		_player = player;
		_skill = skill;
		_type = type;
	}

	public Npc? getTrainer()
	{
		return _trainer;
	}

	public Player getPlayer()
	{
		return _player;
	}

	public Skill getSkill()
	{
		return _skill;
	}

	public AcquireSkillType getAcquireType()
	{
		return _type;
	}
}