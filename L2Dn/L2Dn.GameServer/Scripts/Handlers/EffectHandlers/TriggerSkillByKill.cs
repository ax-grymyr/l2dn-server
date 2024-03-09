using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Trigger Skill By Kill effect implementation.
 * @author Sdw
 */
public class TriggerSkillByKill: AbstractEffect
{
	private readonly int _chance;
	private readonly SkillHolder _skill;
	
	public TriggerSkillByKill(StatSet @params)
	{
		_chance = @params.getInt("chance", 100);
		_skill = new SkillHolder(@params.getInt("skillId", 0), @params.getInt("skillLevel", 0));
	}
	
	private void onCreatureKilled(OnCreatureKilled @event, Creature target)
	{
		if ((_chance == 0) || ((_skill.getSkillId() == 0) || (_skill.getSkillLevel() == 0)))
		{
			return;
		}
		
		if (Rnd.get(100) > _chance)
		{
			return;
		}
		
		Skill triggerSkill = _skill.getSkill();
		
		if (@event.getAttacker() == target)
		{
			SkillCaster.triggerCast(target, target, triggerSkill);
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeListenerIf(EventType.ON_CREATURE_KILLED, listener => listener.getOwner() == this);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.addListener(new ConsumerEventListener(effected, EventType.ON_CREATURE_KILLED,
			@event => onCreatureKilled((OnCreatureKilled)@event, effected), this));
	}
}