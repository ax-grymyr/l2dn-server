using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Block Actions effect implementation.
 * @author mkizub
 */
public class BlockActions: AbstractEffect
{
	private readonly Set<int> _allowedSkills = new();
	
	public BlockActions(StatSet @params)
	{
		foreach (String skill in @params.getString("allowedSkills", "").Split(";"))
		{
			if (!string.IsNullOrEmpty(skill))
			{
				_allowedSkills.add(int.Parse(skill));
			}
		}
	}
	
	public override long getEffectFlags()
	{
		return _allowedSkills.isEmpty() ? EffectFlag.BLOCK_ACTIONS.getMask() : EffectFlag.CONDITIONAL_BLOCK_ACTIONS.getMask();
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.BLOCK_ACTIONS;
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((effected == null) || effected.isRaid())
		{
			return;
		}
		
		foreach (int skillId in _allowedSkills)
		{
			effected.addBlockActionsAllowedSkill(skillId);
		}
		
		effected.startParalyze();
		
		// Cancel running skill casters.
		effected.abortAllSkillCasters();
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		foreach (int skillId in _allowedSkills)
		{
			effected.removeBlockActionsAllowedSkill(skillId);
		}
		
		if (effected.isPlayable())
		{
			if (effected.isSummon())
			{
				if ((effector != null) && !effector.isDead())
				{
					if (effector.isPlayable() && (effected.getActingPlayer().getPvpFlag() == PvpFlagStatus.None))
					{
						effected.disableCoreAI(false);
					}
					else
					{
						((L2Dn.GameServer.Model.Actor.Summon)effected).doAutoAttack(effector);
					}
				}
				else
				{
					effected.disableCoreAI(false);
				}
			}
			else
			{
				effected.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			}
		}
		else
		{
			effected.getAI().notifyEvent(CtrlEvent.EVT_THINK);
		}
	}
}