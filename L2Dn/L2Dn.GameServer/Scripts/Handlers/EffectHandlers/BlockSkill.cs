using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Events.Returns;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Block Skills by isMagic type or skill id.
 * @author Nik, Mobius
 */
public class BlockSkill: AbstractEffect
{
	private readonly Set<int> _magicTypes = new();
	private readonly Set<int> _skillIds = new();
	
	public BlockSkill(StatSet @params)
	{
		if (@params.contains("magicTypes"))
		{
			foreach (int id in @params.getIntArray("magicTypes", ";"))
			{
				_magicTypes.add(id);
			}
		}
		if (@params.contains("skillIds"))
		{
			foreach (int id in @params.getIntArray("skillIds", ";"))
			{
				_skillIds.add(id);
			}
		}
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_magicTypes.isEmpty() && _skillIds.isEmpty())
		{
			return;
		}
		
		effected.addListener(new FunctionEventListener(effected, EventType.ON_CREATURE_SKILL_USE, @event => onSkillUseEvent((OnCreatureSkillUse)@event), this));
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeListenerIf(EventType.ON_CREATURE_SKILL_USE, listener => listener.getOwner() == this);
	}
	
	private TerminateReturn onSkillUseEvent(OnCreatureSkillUse @event)
	{
		if (_magicTypes.Contains(@event.getSkill().getMagicType()) || _skillIds.Contains(@event.getSkill().getId()))
		{
			return new TerminateReturn(true, true, true);
		}
		
		return null;
	}
}