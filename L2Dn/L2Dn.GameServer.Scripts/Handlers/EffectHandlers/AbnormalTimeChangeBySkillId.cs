using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class AbnormalTimeChangeBySkillId: AbstractEffect
{
	private readonly double _time;
	private readonly StatModifierType _mode;
	private readonly Set<int> _skillIds = [];

	public AbnormalTimeChangeBySkillId(StatSet @params)
	{
		_time = @params.getDouble("time", -1);
		_mode = @params.getEnum("mode", StatModifierType.PER);
		string skillIds = @params.getString("ids", string.Empty);
		if (!string.IsNullOrEmpty(skillIds))
		{
			foreach (string id in skillIds.Split(","))
			{
				_skillIds.add(int.Parse(id));
			}
		}
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.Events.Subscribe<OnCreatureSkillUse>(this, onCreatureSkillUse);
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.Events.Unsubscribe<OnCreatureSkillUse>(onCreatureSkillUse);
	}

	private void onCreatureSkillUse(OnCreatureSkillUse @event)
	{
		Skill skill = @event.getSkill();
		if (!_skillIds.Contains(skill.getId()))
		{
			return;
		}

		AbnormalStatusUpdatePacket asu = new AbnormalStatusUpdatePacket([]);
		Creature creature = @event.getCaster();
		foreach (BuffInfo info in creature.getEffectList().getEffects())
		{
			if (info.getSkill().getId() == skill.getId())
			{
				if (_mode == StatModifierType.PER)
				{
					info.resetAbnormalTime(info.getAbnormalTime() * _time);
				}
				else // DIFF
				{
					info.resetAbnormalTime(info.getAbnormalTime() + TimeSpan.FromSeconds(_time));
				}
				asu.addSkill(info);
			}
		}

		creature.sendPacket(asu);
	}
}