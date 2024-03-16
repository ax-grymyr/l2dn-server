using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
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
		
		effected.Events.Subscribe<OnCreatureSkillUse>(this, onSkillUseEvent);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.Events.Unsubscribe<OnCreatureSkillUse>(onSkillUseEvent);
	}

	private void onSkillUseEvent(OnCreatureSkillUse ev)
	{
		if (_magicTypes.Contains(ev.getSkill().getMagicType()) || _skillIds.Contains(ev.getSkill().getId()))
		{
			ev.Terminate = true;
			ev.Abort = true;
		}
	}
}