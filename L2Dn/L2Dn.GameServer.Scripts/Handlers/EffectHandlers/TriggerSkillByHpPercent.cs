using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class TriggerSkillByHpPercent: AbstractEffect
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly int _percentFrom;
	private readonly int _percentTo;
	
	public TriggerSkillByHpPercent(StatSet @params)
	{
		_skillId = @params.getInt("skillId", 0);
		_skillLevel = @params.getInt("skillLevel", 1);
		_percentFrom = @params.getInt("percentFrom", 0);
		_percentTo = @params.getInt("percentTo", 100);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.Events.Subscribe<OnCreatureHpChange>(this, onHpChange);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.Events.Unsubscribe<OnCreatureHpChange>(onHpChange);
	}
	
	private void onHpChange(OnCreatureHpChange @event)
	{
		Creature creature = @event.getCreature();
		int hpPercent = creature.getCurrentHpPercent();
		if ((hpPercent >= _percentFrom) && (hpPercent <= _percentTo))
		{
			if (!creature.isAffectedBySkill(_skillId))
			{
				SkillCaster.triggerCast(creature, creature, SkillData.getInstance().getSkill(_skillId, _skillLevel));
			}
		}
		else
		{
			creature.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _skillId);
		}
	}
}